using System.ComponentModel.DataAnnotations;

namespace Ribera2App.API.DTOs
{
    public class RegistroUsuarioDto
    {
        [Required]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = "residente"; // Valor por defecto
    }
}