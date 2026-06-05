using Ribera2App.API.Data;
using Ribera2App.API.DTOs;
using Ribera2App.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting; // Necesario para IWebHostEnvironment
using System.IO; // Necesario para Path y FileStream

namespace Ribera2App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Todo este módulo requiere login
    public class PagosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env; // Para saber la ruta del servidor

        // Constructor actualizado
        public PagosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env; // Se inyecta el servicio de Environment
        }

        // --- ENDPOINT 1: RESIDENTE REPORTA SU PAGO ---
        // POST: api/Pagos/reportar
        [HttpPost("reportar")]
        [Authorize(Roles = "residente")] // Solo residentes reportan
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ReportarPago([FromForm] PagoReporteDto reporteDto)
        {
            // Forma más segura de obtener el ID
            var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim, out var idUsuario))
            {
                return Unauthorized(new { message = "Token inválido." });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validar que el monto reportado coincida con las deudas
                decimal montoDeudas = await _context.Cobros
                    .Where(c => reporteDto.IdsCobrosPagados.Contains(c.IdCobro) && c.Estado == "pendiente") // <-- Solo sumar pendientes
                    .SumAsync(c => c.Monto);

                if (montoDeudas == 0)
                {
                    return BadRequest(new { message = "Las deudas seleccionadas ya han sido pagadas o están en validación." });
                }

                if (reporteDto.MontoTransferido != montoDeudas)
                {
                    await transaction.RollbackAsync(); // Revertimos antes de salir
                    return BadRequest(new { message = $"El monto transferido (${reporteDto.MontoTransferido}) no coincide con la suma de las deudas (${montoDeudas})." });
                }

                // --- Lógica para guardar el archivo ---
                string? urlComprobanteUnica = null; // La URL que guardaremos en la BD
                if (reporteDto.Comprobante != null && reporteDto.Comprobante.Length > 0)
                {
                    string uploadsDir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                    string extension = Path.GetExtension(reporteDto.Comprobante.FileName);
                    string nombreUnico = $"{Guid.NewGuid()}{extension}";
                    string filePath = Path.Combine(uploadsDir, nombreUnico);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await reporteDto.Comprobante.CopyToAsync(fileStream);
                    }
                    urlComprobanteUnica = $"/uploads/{nombreUnico}";
                }
                // --- FIN DE LÓGICA DE ARCHIVO ---

                // 2. Crear el ReporteDePago principal
                var nuevoReporte = new ReporteDePago
                {
                    IdUsuarioReporta = idUsuario,
                    FechaTransferencia = reporteDto.FechaTransferencia,
                    MontoTransferido = reporteDto.MontoTransferido,
                    NumeroReferencia = reporteDto.NumeroReferencia,
                    UrlComprobante = urlComprobanteUnica,
                    EstadoValidacion = "en_revision",
                    FechaCreacion = DateTime.Now
                };
                _context.ReportesDePago.Add(nuevoReporte);
                await _context.SaveChangesAsync(); // Guardamos para obtener el ID

                // 3. Vincular el reporte con las deudas en la tabla pivote
                foreach (var idCobro in reporteDto.IdsCobrosPagados)
                {
                    var link = new Pagos_vs_Cobros
                    {
                        IdReporteAprobado = nuevoReporte.IdReporte,
                        IdCobroPagado = idCobro
                    };
                    _context.Pagos_vs_Cobros.Add(link);
                }
                await _context.SaveChangesAsync();

                // --- INICIO DE CÓDIGO NUEVO (Lógica Impecable) ---
                // 4. Marcar todas las deudas (Cobros) como "en_validacion"
                await _context.Cobros
                    .Where(c => reporteDto.IdsCobrosPagados.Contains(c.IdCobro))
                    .ExecuteUpdateAsync(s => s.SetProperty(c => c.Estado, "en_validacion"));
                // --- FIN DE CÓDIGO NUEVO ---

                // 5. Si todo salió bien, confirmamos la transacción
                await transaction.CommitAsync();

                return Ok(new { message = "Pago reportado exitosamente. Queda pendiente de validación." });
            }
            catch (Exception ex)
            {
                // 6. Si algo falló, revertimos todo
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error al reportar el pago.", error = ex.Message });
            }
        }

        // --- ENDPOINT 2: ADMIN VE PAGOS PENDIENTES ---
        // GET: api/Pagos/pendientes
        [HttpGet("pendientes")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPagosPendientes()
        {
            var reportes = await _context.ReportesDePago
                .Where(r => r.EstadoValidacion == "en_revision")
                .Include(r => r.UsuarioReporta) // Incluir datos del usuario
                .Select(r => new
                {
                    r.IdReporte,
                    Usuario = r.UsuarioReporta != null ? r.UsuarioReporta.NombreCompleto : "N/A",
                    r.FechaTransferencia,
                    r.MontoTransferido,
                    r.NumeroReferencia,
                    r.UrlComprobante // El admin necesita ver este link
                })
                .ToListAsync();

            return Ok(reportes);
        }

        // --- ENDPOINT 3: ADMIN VALIDA UN PAGO ---
        // POST: api/Pagos/validar/{idReporte}
        [HttpPost("validar/{idReporte}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ValidarPago(int idReporte)
        {
            // Forma más segura de obtener el ID del Admin
            var idAdminClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idAdminClaim == null || !int.TryParse(idAdminClaim, out var idAdmin))
            {
                return Unauthorized(new { message = "Token de admin inválido." });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Buscar el reporte
                var reporte = await _context.ReportesDePago.FindAsync(idReporte);
                if (reporte == null || reporte.EstadoValidacion != "en_revision")
                {
                    return NotFound(new { message = "Reporte no encontrado o ya fue validado." });
                }

                // 2. Marcar el reporte como "aprobado"
                reporte.EstadoValidacion = "aprobado";
                reporte.IdAdminValida = idAdmin;

                // 3. Buscar todas las deudas (Cobros) vinculadas a este reporte
                var idsDeudas = await _context.Pagos_vs_Cobros
                    .Where(pc => pc.IdReporteAprobado == idReporte)
                    .Select(pc => pc.IdCobroPagado)
                    .ToListAsync();

                // 4. Marcar todas esas deudas como "pagado"
                await _context.Cobros
                    .Where(c => idsDeudas.Contains(c.IdCobro))
                    .ExecuteUpdateAsync(s => s.SetProperty(c => c.Estado, "pagado"));

                // 5. Guardar cambios y confirmar transacción
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Pago validado y deudas saldadas exitosamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error al validar el pago.", error = ex.Message });
            }
        }

        // --- ENDPOINT 4: ADMIN RECHAZA UN PAGO (¡NUEVO!) ---
        // POST: api/Pagos/rechazar/{idReporte}
        [HttpPost("rechazar/{idReporte}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RechazarPago(int idReporte)
        {
            var idAdminClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idAdminClaim == null || !int.TryParse(idAdminClaim, out var idAdmin))
            {
                return Unauthorized(new { message = "Token de admin inválido." });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Buscar el reporte
                var reporte = await _context.ReportesDePago.FindAsync(idReporte);
                if (reporte == null || reporte.EstadoValidacion != "en_revision")
                {
                    return NotFound(new { message = "Reporte no encontrado o ya fue procesado." });
                }

                // 2. Marcar el reporte como "rechazado"
                reporte.EstadoValidacion = "rechazado";
                reporte.IdAdminValida = idAdmin;

                // 3. Buscar todas las deudas (Cobros) vinculadas a este reporte
                var idsDeudas = await _context.Pagos_vs_Cobros
                    .Where(pc => pc.IdReporteAprobado == idReporte)
                    .Select(pc => pc.IdCobroPagado)
                    .ToListAsync();

                // 4. DEVOLVER todas esas deudas al estado "pendiente"
                await _context.Cobros
                    .Where(c => idsDeudas.Contains(c.IdCobro) && c.Estado == "en_validacion")
                    .ExecuteUpdateAsync(s => s.SetProperty(c => c.Estado, "pendiente"));

                // 5. Guardar cambios y confirmar transacción
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Pago rechazado. Las deudas han sido re-activadas." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error al rechazar el pago.", error = ex.Message });
            }
        }


        // --- ENDPOINT 5: RESIDENTE VE SU HISTORIAL DE PAGOS ---
        // GET: api/Pagos/mis-reportes
        [HttpGet("mis-reportes")]
        [Authorize(Roles = "residente")] // <-- ¡Solo para residentes!
        public async Task<IActionResult> GetMisReportesDePago()
        {
            // 1. Obtener ID del residente desde el token (forma segura)
            var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim, out var idUsuario))
            {
                return Unauthorized(new { message = "Token inválido." });
            }

            // 2. Buscar todos los reportes de este usuario
            var reportes = await _context.ReportesDePago
                .Where(r => r.IdUsuarioReporta == idUsuario) // <-- La magia: filtrar por su ID
                .OrderByDescending(r => r.FechaCreacion) // <-- Mostrar el más reciente primero
                .Select(r => new
                {
                    r.IdReporte,
                    r.FechaCreacion,
                    r.MontoTransferido,
                    r.NumeroReferencia,
                    r.EstadoValidacion, // <-- Para que vea si está "en_revision", "aprobado", etc.
                    r.UrlComprobante
                })
                .ToListAsync();

            return Ok(reportes);
        }
    }

}