using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StoreApp.DAL.Context;
using StoreApp.Domain.Enums;

namespace StoreApp.WebApi.Infrastructure.BackgroundServices;

public class ExpiredTicketCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExpiredTicketCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public ExpiredTicketCleanupService(
        IServiceProvider serviceProvider,
        ILogger<ExpiredTicketCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExpiredTicketCleanupService başladı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredTicketsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vaxtı keçmiş biletlər silinərkən xəta baş verdi.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CleanupExpiredTicketsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreAppDbContext>();

        var now = DateTime.UtcNow;

        var expiredPlaneTickets = await dbContext.PlaneTickets
            .Where(t => t.CustomerId == null
                     && (t.State == State.Pending || t.State == State.Available)
                     && t.DueDate < now)
            .ToListAsync(cancellationToken);

        int deletedPlane = 0;
        int deletedPlaneSeats = 0;

        if (expiredPlaneTickets.Any())
        {
            var planeIds = expiredPlaneTickets.Select(t => t.Id).ToList();

            var planeSeats = await dbContext.Seats
                .Where(s => s.PlaneTicketId != null && planeIds.Contains(s.PlaneTicketId.Value))
                .ToListAsync(cancellationToken);

            if (planeSeats.Any())
                dbContext.Seats.RemoveRange(planeSeats);

            dbContext.PlaneTickets.RemoveRange(expiredPlaneTickets);

            deletedPlane = expiredPlaneTickets.Count;
            deletedPlaneSeats = planeSeats.Count;
        }

        var expiredTrainTickets = await dbContext.TrainTickets
            .Where(t => t.CustomerId == null
                     && (t.State == State.Pending || t.State == State.Available)
                     && t.DueDate < now)
            .ToListAsync(cancellationToken);

        int deletedTrain = 0;
        int deletedTrainSeats = 0;

        if (expiredTrainTickets.Any())
        {
            var trainIds = expiredTrainTickets.Select(t => t.Id).ToList();

            var trainSeats = await dbContext.Seats
                .Where(s => s.TrainTicketId != null && trainIds.Contains(s.TrainTicketId.Value))
                .ToListAsync(cancellationToken);

            if (trainSeats.Any())
                dbContext.Seats.RemoveRange(trainSeats);

            dbContext.TrainTickets.RemoveRange(expiredTrainTickets);

            deletedTrain = expiredTrainTickets.Count;
            deletedTrainSeats = trainSeats.Count;
        }

        if (deletedPlane > 0 || deletedTrain > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Təmizlik tamamlandı — Uçuş: {PlaneCount} bilet, {PlaneSeats} oturacaq | " +
                "Qatар: {TrainCount} bilet, {TrainSeats} oturacaq silindi.",
                deletedPlane, deletedPlaneSeats,
                deletedTrain, deletedTrainSeats);
        }
        else
        {
            _logger.LogInformation("Silinəcək vaxtı keçmiş bilet tapılmadı.");
        }
    }
}