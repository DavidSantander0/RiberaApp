using System.ComponentModel.DataAnnotations;

namespace Ribera2App.API.DTOs
{
    public class PagoReporteDto
    {
        [Required]
        public DateTime FechaTransferencia { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal MontoTransferido { get; set; }

        [Required]
        public string NumeroReferencia { get; set; } = string.Empty;

        // --- CAMBIO AQUÍ ---
        // Ya no es un 'string'. Ahora es el archivo en sí.
        // Puede ser nulo (?), por si no quiere subirlo.
        public IFormFile? Comprobante { get; set; }
        // --- FIN DEL CAMBIO ---

        [Required]
        [MinLength(1)]
        public List<int> IdsCobrosPagados { get; set; } = new List<int>();
    }
}