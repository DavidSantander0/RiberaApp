using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <--- AÑADIR

namespace Ribera2App.API.Models
{
    [Table("ReportesDePago")] // <--- AÑADIR
    public class ReporteDePago
    {
        [Key]
        [Column("id_reporte")] // <--- AÑADIR
        public int IdReporte { get; set; }

        [Required]
        [Column("id_usuario_reporta")] // <--- AÑADIR
        public int IdUsuarioReporta { get; set; }

        [Required]
        [Column("fecha_transferencia")] // <--- AÑADIR
        public DateTime FechaTransferencia { get; set; }

        [Required]
        [Column("monto_transferido", TypeName = "decimal(10, 2)")] // <--- MODIFICAR
        public decimal MontoTransferido { get; set; }

        [Required]
        [StringLength(100)]
        [Column("numero_referencia")] // <--- AÑADIR
        public string NumeroReferencia { get; set; } = string.Empty;

        [Column("url_comprobante")] // <--- AÑADIR
        public string? UrlComprobante { get; set; }

        [Required]
        [StringLength(20)]
        [Column("estado_validacion")] // <--- AÑADR
        public string EstadoValidacion { get; set; } = "en_revision";

        [Column("id_admin_valida")] // <--- AÑADIR
        public int? IdAdminValida { get; set; }

        [Column("fecha_creacion")] // <--- AÑADIR
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [ForeignKey("IdUsuarioReporta")]
        public virtual Usuario? UsuarioReporta { get; set; }
    }
}
