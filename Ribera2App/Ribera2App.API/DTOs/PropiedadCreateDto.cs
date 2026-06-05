using System.ComponentModel.DataAnnotations;

namespace Ribera2App.API.DTOs
{
    public class PropiedadCreateDto
    {
        [Required]
        public string NumeroCasa { get; set; } = string.Empty;

        [Required]
        public int IdPropietario { get; set; }
    }
}