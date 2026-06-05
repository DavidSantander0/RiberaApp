using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <--- AÑADIR

namespace Ribera2App.API.Models
{
    [Table("Cobros")] // <--- AÑADIR
    public class Cobro
    {
        [Key]
        [Column("id_cobro")] // <--- AÑADIR
        public int IdCobro { get; set; }

        [Required]
        [Column("id_propiedad")] // <--- AÑADIR
        public int IdPropiedad { get; set; }

        [Required]
        [StringLength(20)]
        [Column("tipo_cobro")] // <--- AÑADIR
        public string TipoCobro { get; set; } = string.Empty;

        [Required]
        [Column("monto", TypeName = "decimal(10, 2)")] // <--- MODIFICAR
        public decimal Monto { get; set; }

        [Required]
        [StringLength(255)]
        [Column("descripcion")] // <--- AÑADIR
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("estado")] // <--- AÑADIR
        public string Estado { get; set; } = string.Empty;

        [Column("fecha_creacion")] // <--- AÑADIR
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("fecha_vencimiento")] // <--- AÑADIR
        public DateTime? FechaVencimiento { get; set; }

        [ForeignKey("IdPropiedad")]
        public virtual Propiedad? Propiedad { get; set; }
    }
}