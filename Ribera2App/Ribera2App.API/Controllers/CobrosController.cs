using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ribera2App.API.Data;
using Ribera2App.API.DTOs;
using Ribera2App.API.Models;
using System.Security.Claims;

namespace Ribera2App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // <-- ¡Todo este controlador requiere estar logueado!
    public class CobrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CobrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ENDPOINT 1: GENERAR ALÍCUOTAS (Solo Admin) ---
        // POST: api/Cobros/generar-alicuotas
        [HttpPost("generar-alicuotas")]
        [Authorize(Roles = "admin")] // <-- Solo el admin puede ejecutar esto
        public async Task<IActionResult> GenerarAlicuotasMensuales()
        { // <-- SE AÑADIÓ LA LLAVE
            // 1. Definir el cobro
            string descripcionCobro = $"Alícuota del mes {DateTime.Now:MMMM yyyy}";
            decimal montoCobro = 55.00m; // 'm' para decimal

            // 2. Obtener todas las propiedades "activas"
            var propiedadesActivas = await _context.Propiedades
                .Where(p => p.Estado == "activa")
                .ToListAsync();

            int cobrosCreados = 0;
            foreach (var propiedad in propiedadesActivas)
            {
                // 3. (Importante) Verificar si ya se le generó este cobro
                bool yaExiste = await _context.Cobros
                    .AnyAsync(c => c.IdPropiedad == propiedad.IdPropiedad &&
                                   c.Descripcion == descripcionCobro);

                if (!yaExiste)
                {
                    // 4. Si no existe, crear el nuevo cobro
                    var nuevoCobro = new Cobro
                    {
                        IdPropiedad = propiedad.IdPropiedad,
                        TipoCobro = "alicuota",
                        Monto = montoCobro,
                        Descripcion = descripcionCobro,
                        Estado = "pendiente",
                        FechaCreacion = DateTime.Now,
                        FechaVencimiento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1) // Fin de mes
                    };
                    _context.Cobros.Add(nuevoCobro);
                    cobrosCreados++;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Proceso completado. Se generaron {cobrosCreados} nuevas alícuotas." });
        } // <-- SE AÑADIÓ LA LLAVE

        // --- ENDPOINT 3: CREAR MULTA MANUAL (Solo Admin) ---
        // POST: api/Cobros/crear-multa
        [HttpPost("crear-multa")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CrearMulta([FromBody] MultaCreateDto multaDto)
        {
            // 1. Validar que la propiedad exista
            var propiedad = await _context.Propiedades.FindAsync(multaDto.IdPropiedad);
            if (propiedad == null)
            {
                return BadRequest(new { message = "La propiedad no existe." });
            }

            // 2. Crear el nuevo cobro de tipo "multa"
            var nuevaMulta = new Cobro
            {
                IdPropiedad = multaDto.IdPropiedad,
                TipoCobro = "multa",
                Monto = multaDto.Monto,
                Descripcion = multaDto.Descripcion,
                Estado = "pendiente",
                FechaCreacion = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(30) // Dar 30 días para pagar la multa
            };

            _context.Cobros.Add(nuevaMulta);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Multa creada exitosamente." });
        }

        // --- ENDPOINT 2: VER MIS DEUDAS (Residente o Admin) - ¡ACTUALIZADO Y MÁS SEGURO! ---
        // GET: api/Cobros/mis-deudas
        [HttpGet("mis-deudas")]
        [Authorize(Roles = "admin,residente")]
        public async Task<IActionResult> GetMisDeudas()
        {
            // 1. Obtener el ID del usuario desde el token (Forma segura)
            var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rolUsuario = User.FindFirstValue(ClaimTypes.Role);

            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim, out var idUsuario))
            {
                return Unauthorized(new { message = "Token inválido o corrupto." });
            }

            // Incluir Propiedad para acceder a sus propiedades
            IQueryable<Cobro> query = _context.Cobros.Include(c => c.Propiedad);

            if (rolUsuario == "residente")
            {
                // Filtrar solo si Propiedad NO es nulo Y IdPropietario coincide
                query = query.Where(c => c.Propiedad != null && c.Propiedad.IdPropietario == idUsuario);
            }

            var deudas = await query
                .OrderBy(c => c.FechaCreacion)
                .Select(c => new
                {
                    c.IdCobro,
                    // Verificar si Propiedad es nulo antes de acceder a NumeroCasa
                    NumeroCasa = c.Propiedad != null ? c.Propiedad.NumeroCasa : "N/A",
                    c.TipoCobro,
                    c.Descripcion,
                    c.Monto,
                    c.FechaVencimiento,
                    c.Estado // <-- Enviamos el estado
                })
                .ToListAsync();

            return Ok(deudas);
        }
    }
}