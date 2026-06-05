using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <--- AÑADIR

namespace Ribera2App.API.Models
{
    [Table("Usuarios")] // Le dice el nombre de la tabla
    public class Usuario
    {
        [Key]
        [Column("id_usuario")] // <--- AÑADIR
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(150)]
        [Column("nombre_completo")] // <--- AÑADIR
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("email")] // <--- AÑADIR (buena práctica)
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")] // <--- AÑADIR
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("rol")] // <--- AÑADIR (buena práctica)
        public string Rol { get; set; } = string.Empty;

        public virtual ICollection<Propiedad> Propiedades { get; set; } = new List<Propiedad>();
    }
}
