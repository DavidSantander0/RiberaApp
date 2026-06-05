using System.ComponentModel.DataAnnotations.Schema; // <--- AÑADIR

namespace Ribera2App.API.Models
{
    [Table("Pagos_vs_Cobros")] // <--- AÑADIR
    public class Pagos_vs_Cobros
    {
        [Column("id_reporte_aprobado")] // <--- AÑADIR
        public int IdReporteAprobado { get; set; }

        [Column("id_cobro_pagado")] // <--- AÑADIR
        public int IdCobroPagado { get; set; }

        [ForeignKey("IdReporteAprobado")]
        public virtual ReporteDePago? ReporteDePago { get; set; }

        [ForeignKey("IdCobroPagado")]
        public virtual Cobro? Cobro { get; set; }
    }
}