using System.ComponentModel.DataAnnotations;

namespace Ribera2App.API.DTOs
{
    public class MultaCreateDto
    {
        [Required]
        public int IdPropiedad { get; set; }

        [Required]
        [Range(0.01, 10000)] // El monto debe ser positivo
        public decimal Monto { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripcion { get; set; } = string.Empty;
    }
}