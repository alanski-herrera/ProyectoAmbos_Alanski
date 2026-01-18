using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProyectoAmbos_Alanski.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
public class ReservationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReservationCleanupService> _logger;
    public ReservationCleanupService(IServiceProvider serviceProvider, ILogger<ReservationCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Servicio de Limpieza de Reservas iniciado.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Reemplaza con tu DbContext real
                    // Tiempo de expiración: 30 minutos
                    var tiempoExpiracion = DateTime.Now.AddMinutes(-30);
                    // Buscar productos reservados hace más de 30 mins
                    var reservasVencidas = context.Uniformes
                        .Where(u => u.Estado == "Reservado" && u.FechaReserva < tiempoExpiracion)
                        .ToList();
                    if (reservasVencidas.Any())
                    {
                        foreach (var uniforme in reservasVencidas)
                        {
                            uniforme.Estado = "Disponible";
                            uniforme.FechaReserva = null;
                        }
                        await context.SaveChangesAsync();
                        _logger.LogInformation($"Se liberaron {reservasVencidas.Count} reservas vencidas.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando limpieza de reservas.");
            }
            // Esperar 1 minuto antes de la próxima ejecución
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}