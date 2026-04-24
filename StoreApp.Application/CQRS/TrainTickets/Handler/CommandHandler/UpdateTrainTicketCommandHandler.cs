using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

// Tək bilet yeniləmə
public class UpdateTrainTicketCommandHandler : IRequestHandler<UpdateTrainTicketCommandRequest, ResponseModel<UpdateTrainTicketCommandResponse>>
{
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdateTrainTicketCommandHandler> _logger;

    public UpdateTrainTicketCommandHandler(
        StoreAppDbContext db,
        IConfiguration configuration,
        ILogger<UpdateTrainTicketCommandHandler> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<UpdateTrainTicketCommandResponse>> Handle(UpdateTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        // Hədəf bileti tap
        var trainTicket = await _db.TrainTickets
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

        if (trainTicket == null)
            return new ResponseModel<UpdateTrainTicketCommandResponse>(null);

        if (trainTicket.State == State.Used || trainTicket.State == State.Expired || trainTicket.State == State.Missed)
            return new ResponseModel<UpdateTrainTicketCommandResponse>(null);

        // ✅ FIX: Eyni qrupa aid BÜTÜN biletləri tap
        // (eyni TrainCompany + TrainNumber + VagonNumber + DueDate.Date + From + To)
        var groupTickets = await _db.TrainTickets
            .Include(t => t.Customer)
            .Include(t => t.From)
            .Include(t => t.To)
            .Where(t =>
                t.TrainCompany == trainTicket.TrainCompany &&
                t.TrainNumber == trainTicket.TrainNumber &&
                t.VagonNumber == trainTicket.VagonNumber &&
                t.DueDate.Date == trainTicket.DueDate.Date &&
                t.FromId == trainTicket.FromId &&
                t.ToId == trainTicket.ToId &&
                !t.IsDeleted)
            .ToListAsync(cancellationToken);

        var invalidStates = new[] { State.Used, State.Expired, State.Missed };

        // Tarix/saat dəyişikliyi var mı? Email üçün izlə
        bool dateChanged = request.DueDate.HasValue && request.DueDate.Value != trainTicket.DueDate;
        DateTime oldDueDate = trainTicket.DueDate;

        // Email göndəriləcək Booked müştərilər
        var notifyList = new List<(string Email, string FullName, string From, string To, DateTime OldDate, DateTime NewDate)>();

        foreach (var ticket in groupTickets)
        {
            // Used/Expired/Missed biletlərə toxunma
            if (invalidStates.Contains(ticket.State))
                continue;

            // Cancel əməliyyatı — oturacağı azad et
            if (request.State == State.Canceled && ticket.State != State.Canceled)
            {
                if (ticket.ChosenSeatId.HasValue)
                {
                    var seat = await _db.Seats.FirstOrDefaultAsync(s => s.Id == ticket.ChosenSeatId.Value, cancellationToken);
                    if (seat != null)
                        seat.IsOccupied = false;
                }
                ticket.CustomerId = null;
            }

            // Booked biletdə state dəyişmə (yalnız tarix/şirkət/nömrə update et)
            if (ticket.State == State.Booked && request.State != State.Canceled)
            {
                // Tarix dəyişibsə və bilet alınıbsa email siyahısına əlavə et
                if (dateChanged && ticket.Customer != null && !string.IsNullOrEmpty(ticket.Customer.Email))
                {
                    notifyList.Add((
                        ticket.Customer.Email,
                        $"{ticket.Customer.Name} {ticket.Customer.Surname}".Trim(),
                        ticket.From?.Name ?? "N/A",
                        ticket.To?.Name ?? "N/A",
                        oldDueDate,
                        request.DueDate!.Value
                    ));
                }

                ticket.TrainCompany = request.TrainCompany;
                ticket.TrainNumber = request.TrainNumber;
                ticket.VagonNumber = request.VagonNumber;
                if (request.DueDate.HasValue)
                    ticket.DueDate = request.DueDate.Value;
                ticket.UpdatedDate = DateTime.UtcNow;
                continue;
            }

            // Normal update
            ticket.TrainCompany = request.TrainCompany;
            ticket.TrainNumber = request.TrainNumber;
            ticket.VagonNumber = request.VagonNumber;
            ticket.State = request.State;
            if (request.DueDate.HasValue)
                ticket.DueDate = request.DueDate.Value;
            ticket.UpdatedDate = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        // Email bildirişlərini arxa fonda göndər
        if (notifyList.Any())
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var appPassword = _configuration["EmailSettings:AppPassword"];
            var emailService = new Service.Email();

            _ = Task.Run(() =>
            {
                foreach (var n in notifyList)
                {
                    try
                    {
                        emailService.Send(
                            senderEmail,
                            appPassword,
                            n.Email,
                            "⚠️ Train Ticket Schedule Change – StepTravel",
                            $@"
                                <p>Dear <strong>{n.FullName}</strong>,</p>
                                <p>We would like to inform you that your train ticket schedule has been updated.</p>
                                <br/>
                                <table style='border-collapse:collapse;'>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Route</td>
                                        <td><strong>{n.From} → {n.To}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Previous Departure</td>
                                        <td><strong style='color:#c0392b;'>{n.OldDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>New Departure</td>
                                        <td><strong style='color:#27ae60;'>{n.NewDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                                </table>
                                <br/>
                                <p style='color:#555;font-size:13px;'>Please check your updated ticket in the StepTravel app. We apologize for any inconvenience.</p>
                            "
                        );
                        _logger.LogInformation("Schedule change email sent to: {Email}", n.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send schedule change email to {Email}", n.Email);
                    }
                }
            });
        }

        return new ResponseModel<UpdateTrainTicketCommandResponse>(new UpdateTrainTicketCommandResponse
        {
            Id = trainTicket.Id,
            TrainCompany = trainTicket.TrainCompany,
            TrainNumber = trainTicket.TrainNumber,
            VagonNumber = trainTicket.VagonNumber,
            State = trainTicket.State,
            DueDate = trainTicket.DueDate
        });
    }
}

// Qrup yeniləmə
public class UpdateTrainTicketGroupCommandHandler
    : IRequestHandler<UpdateTrainTicketGroupCommandRequest, ResponseModel<List<UpdateTrainTicketCommandResponse>>>
{
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdateTrainTicketGroupCommandHandler> _logger;

    public UpdateTrainTicketGroupCommandHandler(
        StoreAppDbContext db,
        IConfiguration configuration,
        ILogger<UpdateTrainTicketGroupCommandHandler> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<List<UpdateTrainTicketCommandResponse>>> Handle(
        UpdateTrainTicketGroupCommandRequest request, CancellationToken cancellationToken)
    {
        var groupTickets = await _db.TrainTickets
            .Include(t => t.Customer)
            .Include(t => t.From)
            .Include(t => t.To)
            .Where(t =>
                t.TrainCompany == request.TrainCompany &&
                t.TrainNumber == request.TrainNumber &&
                t.VagonNumber == request.VagonNumber &&
                t.DueDate.Date == request.DueDate.Date &&
                t.FromId == request.FromId &&
                t.ToId == request.ToId &&
                !t.IsDeleted)
            .ToListAsync(cancellationToken);

        if (!groupTickets.Any())
            return new ResponseModel<List<UpdateTrainTicketCommandResponse>>(null);

        var invalidStates = new[] { State.Used, State.Expired, State.Missed };
        bool dateChanged = request.NewDueDate.HasValue && request.NewDueDate.Value.Date != request.DueDate.Date;
        DateTime oldDueDate = request.DueDate;
        var notifyList = new List<(string Email, string FullName, string From, string To, DateTime OldDate, DateTime NewDate)>();

        foreach (var ticket in groupTickets)
        {
            if (invalidStates.Contains(ticket.State))
                continue;

            if (ticket.State == State.Booked && request.NewState != State.Canceled)
            {
                // Tarix dəyişibsə Booked müştərilərə bildiriş
                if (dateChanged && ticket.Customer != null && !string.IsNullOrEmpty(ticket.Customer.Email))
                {
                    notifyList.Add((
                        ticket.Customer.Email,
                        $"{ticket.Customer.Name} {ticket.Customer.Surname}".Trim(),
                        ticket.From?.Name ?? "N/A",
                        ticket.To?.Name ?? "N/A",
                        oldDueDate,
                        request.NewDueDate!.Value
                    ));
                }

                ticket.TrainCompany = request.NewTrainCompany;
                ticket.TrainNumber = request.NewTrainNumber;
                ticket.VagonNumber = request.NewVagonNumber;
                if (request.NewDueDate.HasValue)
                    ticket.DueDate = request.NewDueDate.Value;
                ticket.UpdatedDate = DateTime.UtcNow;
                continue;
            }

            if (request.NewState == State.Canceled && ticket.State != State.Canceled)
            {
                if (ticket.ChosenSeatId.HasValue)
                {
                    var seat = await _db.Seats.FirstOrDefaultAsync(s => s.Id == ticket.ChosenSeatId.Value, cancellationToken);
                    if (seat != null)
                        seat.IsOccupied = false;
                }
                ticket.CustomerId = null;
            }

            ticket.TrainCompany = request.NewTrainCompany;
            ticket.TrainNumber = request.NewTrainNumber;
            ticket.VagonNumber = request.NewVagonNumber;
            ticket.State = request.NewState;
            if (request.NewDueDate.HasValue)
                ticket.DueDate = request.NewDueDate.Value;
            ticket.UpdatedDate = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        // Email bildirişlərini arxa fonda göndər
        if (notifyList.Any())
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var appPassword = _configuration["EmailSettings:AppPassword"];
            var emailService = new Service.Email();

            _ = Task.Run(() =>
            {
                foreach (var n in notifyList)
                {
                    try
                    {
                        emailService.Send(
                            senderEmail,
                            appPassword,
                            n.Email,
                            "⚠️ Train Ticket Schedule Change – StepTravel",
                            $@"
                                <p>Dear <strong>{n.FullName}</strong>,</p>
                                <p>We would like to inform you that your train ticket schedule has been updated.</p>
                                <br/>
                                <table style='border-collapse:collapse;'>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Route</td>
                                        <td><strong>{n.From} → {n.To}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Previous Departure</td>
                                        <td><strong style='color:#c0392b;'>{n.OldDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>New Departure</td>
                                        <td><strong style='color:#27ae60;'>{n.NewDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                                </table>
                                <br/>
                                <p style='color:#555;font-size:13px;'>Please check your updated ticket in the StepTravel app. We apologize for any inconvenience.</p>
                            "
                        );
                        _logger.LogInformation("Group schedule change email sent to: {Email}", n.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send group schedule change email to {Email}", n.Email);
                    }
                }
            });
        }

        return new ResponseModel<List<UpdateTrainTicketCommandResponse>>(
            groupTickets.Select(t => new UpdateTrainTicketCommandResponse
            {
                Id = t.Id,
                TrainCompany = t.TrainCompany,
                TrainNumber = t.TrainNumber,
                VagonNumber = t.VagonNumber,
                State = t.State,
                DueDate = t.DueDate
            }).ToList());
    }
}