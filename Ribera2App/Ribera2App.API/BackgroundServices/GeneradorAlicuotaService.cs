using Ribera2App.API.Data;
using Ribera2App.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ribera2App.API.BackgroundServices
{
    // 1. Heredamos de BackgroundService
    public class GeneradorAlicuotasService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GeneradorAlicuotasService> _logger;

        // 2. No podemos inyectar DbContext aquí directamente (es complicado).
        // En su lugar, inyectamos el "Proveedor de Servicios" para poder
        // obtener un DbContext "fresco" cada vez que lo necesitemos.
        public GeneradorAlicuotasService(IServiceProvider serviceProvider, ILogger<GeneradorAlicuotasService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // 3. Este es el método principal del "robot"
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de generación de alícuotas INICIADO.");

            // 4. El robot vivirá por siempre, hasta que se apague la API
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 5. Llamamos a nuestra lógica de trabajo
                    await GenerarAlicuotas(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error en el servicio de alícuotas.");
                }

                // 6. El robot se duerme por 1 hora
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("Servicio de generación de alícuotas DETENIDO.");
        }

        private async Task GenerarAlicuotas(CancellationToken stoppingToken)
        {
            // 7. Nos despertamos y revisamos el calendario
            var ahora = DateTime.Now;

            // 8. ¡La lógica clave!
            // Si HOY NO es el día 1 del mes, no hacemos nada y volvemos a dormir.
            if (ahora.Day != 1)
            {
                // _logger.LogInformation("Hoy no es día 1. No se generan alícuotas.");
                return;
            }

            // 9. Es día 1. Creamos un "scope" para pedir el DbContext
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                _logger.LogInformation("Día 1 detectado. Buscando alícuotas para generar...");

                // --- Esta es la MISMA lógica de tu controlador ---
                string descripcionCobro = $"Alícuota del mes {ahora:MMMM yyyy}";
                decimal montoCobro = 55.00m;

                var propiedadesActivas = await context.Propiedades
                    .Where(p => p.Estado == "activa")
                    .ToListAsync(stoppingToken);

                int cobrosCreados = 0;
                foreach (var propiedad in propiedadesActivas)
                {
                    bool yaExiste = await context.Cobros
                        .AnyAsync(c => c.IdPropiedad == propiedad.IdPropiedad &&
                                       c.Descripcion == descripcionCobro, stoppingToken);

                    if (!yaExiste)
                    {
                        var nuevoCobro = new Cobro
                        {
                            IdPropiedad = propiedad.IdPropiedad,
                            TipoCobro = "alicuota",
                            Monto = montoCobro,
                            Descripcion = descripcionCobro,
                            Estado = "pendiente",
                            FechaCreacion = ahora,
                            FechaVencimiento = new DateTime(ahora.Year, ahora.Month, 1).AddMonths(1).AddDays(-1)
                        };
                        context.Cobros.Add(nuevoCobro);
                        cobrosCreados++;
                    }
                }

                if (cobrosCreados > 0)
                {
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation($"¡ÉXITO! Se generaron {cobrosCreados} nuevas alícuotas.");
                }
                else
                {
                    _logger.LogInformation("No se generaron nuevas alícuotas (probablemente ya existían).");
                }
            }
        }
    }
}
