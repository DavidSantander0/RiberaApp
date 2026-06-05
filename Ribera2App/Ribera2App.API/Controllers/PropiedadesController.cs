using Ribera2App.API.Data;
using Ribera2App.API.DTOs;
using Ribera2App.API.Models;
using Microsoft.AspNetCore.Authorization; // ¡Importante para [Authorize]!
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ribera2App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropiedadesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PropiedadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ENDPOINT OBTENER TODAS LAS PROPIEDADES ---
        // GET: api/Propiedades
        [HttpGet]
        [Authorize] // <-- Requiere CUALQUIER token válido (admin o residente)
        public async Task<IActionResult> GetPropiedades()
        {
            var propiedades = await _context.Propiedades
                .Include(p => p.Propietario) // Incluimos el objeto Propietario
                .Select(p => new { // Devolvemos un objeto limpio
                    p.IdPropiedad,
                    p.NumeroCasa,
                    p.Estado,
                    // Evitamos enviar el objeto de usuario completo
                    Propietario = p.Propietario != null ? p.Propietario.NombreCompleto : "N/A"
                })
                .ToListAsync();

            return Ok(propiedades);
        }

        // --- ENDPOINT CREAR PROPIEDAD ---
        // POST: api/Propiedades
        [HttpPost]
        [Authorize(Roles = "admin")] // <-- ¡LA MAGIA! Solo usuarios con Rol "admin"
        public async Task<IActionResult> CrearPropiedad([FromBody] PropiedadCreateDto propiedadDto)
        {
            // 1. Validar que el ID del propietario exista
            var propietario = await _context.Usuarios.FindAsync(propiedadDto.IdPropietario);
            if (propietario == null)
            {
                return BadRequest(new { message = "El ID del propietario no existe." });
            }

            // 2. Validar que el número de casa no se repita
            if (await _context.Propiedades.AnyAsync(p => p.NumeroCasa == propiedadDto.NumeroCasa))
            {
                return BadRequest(new { message = "El número de casa ya existe." });
            }

            // 3. Crear y guardar la propiedad
            var nuevaPropiedad = new Propiedad
            {
                NumeroCasa = propiedadDto.NumeroCasa,
                IdPropietario = propiedadDto.IdPropietario,
                Estado = "activa" // Estado por defecto
            };

            _context.Propiedades.Add(nuevaPropiedad);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Propiedad creada exitosamente." });
        }
    }
}