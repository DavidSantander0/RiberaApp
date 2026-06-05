using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <--- AÑADIR

namespace Ribera2App.API.Models
{
    [Table("Propiedades")] // <--- AÑADIR
    public class Propiedad
    {
        [Key]
        [Column("id_propiedad")] // <--- AÑADIR
        public int IdPropiedad { get; set; }

        [Required]
        [StringLength(50)]
        [Column("numero_casa")] // <--- AÑADIR
        public string NumeroCasa { get; set; } = string.Empty;

        [Required]
        [Column("id_propietario")] // <--- AÑADIR
        public int IdPropietario { get; set; }

        [Required]
        [StringLength(20)]
        [Column("estado")] // <--- AÑADIR
        public string Estado { get; set; } = string.Empty;

        [ForeignKey("IdPropietario")]
        public virtual Usuario? Propietario { get; set; }

        public virtual ICollection<Cobro> Cobros { get; set; } = new List<Cobro>();
    }
}