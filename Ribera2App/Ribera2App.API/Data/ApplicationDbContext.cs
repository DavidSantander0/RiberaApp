using Ribera2App.API.Models; // Importa tus modelos
using Microsoft.EntityFrameworkCore;

namespace Ribera2App.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Mapeo de Tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Propiedad> Propiedades { get; set; }
        public DbSet<Cobro> Cobros { get; set; }
        public DbSet<ReporteDePago> ReportesDePago { get; set; }
        public DbSet<Pagos_vs_Cobros> Pagos_vs_Cobros { get; set; }

        // Configurar la llave primaria compuesta
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pagos_vs_Cobros>()
                .HasKey(pc => new { pc.IdReporteAprobado, pc.IdCobroPagado });
        }
    }
}