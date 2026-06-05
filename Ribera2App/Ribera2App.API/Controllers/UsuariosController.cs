using Ribera2App.API.Data; // El DbContext
using Ribera2App.API.DTOs; // Los DTOs
using Ribera2App.API.Models; // Los Modelos
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Para .AnyAsync()
using System.Text; // Para Encoding.UTF8
using System.Security.Claims; // Para los "claims" del token
using Microsoft.IdentityModel.Tokens; // Para la llave de seguridad
using System.IdentityModel.Tokens.Jwt; // Para el manejador del token
using Microsoft.AspNetCore.Authorization; // <-- Para [Authorize]

// NOTA: Asegúrate de tener también este 'using' para IConfiguration
using Microsoft.Extensions.Configuration;

namespace Ribera2App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config; // <-- ESTO FALTABA

        // 1. Constructor actualizado para inyectar DbContext y IConfiguration
        public UsuariosController(ApplicationDbContext context, IConfiguration config) // <-- MODIFICADO
        {
            _context = context;
            _config = config; // <-- ESTO FALTABA
        }

        // --- ENDPOINT DE REGISTRO ---
        // POST: api/Usuarios/registro
        [HttpPost("registro")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] RegistroUsuarioDto registroDto)
        {
            // 2. Validar que el email no exista
            if (await _context.Usuarios.AnyAsync(u => u.Email == registroDto.Email))
            {
                return BadRequest(new { message = "El correo electrónico ya está registrado." });
            }

            // 3. Hashear la contraseña (¡Seguridad!)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registroDto.Password);

            // 4. Crear el nuevo objeto Usuario
            var nuevoUsuario = new Usuario
            {
                NombreCompleto = registroDto.NombreCompleto,
                Email = registroDto.Email,
                PasswordHash = passwordHash,
                Rol = registroDto.Rol // "admin" o "residente"
            };

            // 5. Guardar en la Base de Datos
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync(); // Guardamos los cambios

            // 6. Devolver una respuesta exitosa
            return Ok(new { message = "Usuario registrado exitosamente." });
        }

        // --- ENDPOINT DE LOGIN (ACTUALIZADO) ---
        // POST: api/Usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginUsuario([FromBody] LoginDto loginDto)
        {
            // 1. Buscar al usuario por su email
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            // 2. Si el usuario no existe, devolver un error
            if (usuario == null)
            {
                return Unauthorized(new { message = "Email o contraseña incorrectos." });
            }

            // 3. Verificar la contraseña
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash))
            {
                return Unauthorized(new { message = "Email o contraseña incorrectos." });
            }

            // 4. ¡Login exitoso! Crear el Token
            string token = CrearToken(usuario); // <-- MODIFICADO

            return Ok(new
            {
                message = "Login exitoso.",
                token = token, // <-- Devolvemos el token
                usuario = new { usuario.Email, usuario.NombreCompleto, usuario.Rol }
            });
        }
        // --- ENDPOINT PROTEGIDO ---
        // GET: api/Usuarios/perfil
        [HttpGet("perfil")]
        [Authorize] // <-- ¡ESTA ES LA MAGIA! Solo deja pasar si hay un token válido.
        public async Task<IActionResult> GetMiPerfil()
        {
            // Gracias al token, podemos saber QUIÉN está haciendo la llamada.
            // El ID del usuario se guardó en el token con el nombre "NameIdentifier"

            var idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (idUsuario == null)
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == int.Parse(idUsuario));

            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Devolvemos los datos del usuario (sin el hash del password)
            return Ok(new
            {
                usuario.IdUsuario,
                usuario.NombreCompleto,
                usuario.Email,
                usuario.Rol
            });
        }

        // --- MÉTODO PRIVADO PARA CREAR EL TOKEN ---

        private string CrearToken(Usuario usuario)
        {
            // 1. Definir los "Claims" (información que va dentro del token)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            // 2. Obtener la llave secreta desde appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("Jwt:Key").Value!));

            // 3. Crear las credenciales para firmar el token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // 4. Crear la descripción del token (cuánto dura, etc.)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), // El token será válido por 1 día
                SigningCredentials = creds
            };

            // 5. Crear el token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 6. Escribir el token como un string
            return tokenHandler.WriteToken(token);
        }
        // --- ENDPOINT OBTENER RESIDENTES (PARA ADMIN) ---
        // ESTE ES EL MÉTODO QUE TE FALTA O ESTÁ MAL
        // GET: api/Usuarios/residentes
        [HttpGet("residentes")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetResidentes()
        {
            var residentes = await _context.Usuarios
                .Where(u => u.Rol == "residente")
                .Select(u => new
                {
                    u.IdUsuario,
                    u.NombreCompleto,
                    u.Email
                })
                .ToListAsync();

            return Ok(residentes);
        }
    }
}