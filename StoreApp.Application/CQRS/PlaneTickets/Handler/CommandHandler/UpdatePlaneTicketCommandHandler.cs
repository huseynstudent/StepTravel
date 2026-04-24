using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class UpdatePlaneTicketGroupCommandHandler
    : IRequestHandler<UpdatePlaneTicketGroupCommandRequest, ResponseModel<List<UpdatePlaneTicketCommandResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdatePlaneTicketGroupCommandHandler> _logger;

    public UpdatePlaneTicketGroupCommandHandler(
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<UpdatePlaneTicketGroupCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<List<UpdatePlaneTicketCommandResponse>>> Handle(
        UpdatePlaneTicketGroupCommandRequest request, CancellationToken cancellationToken)
    {
        var groupTickets = _unitOfWork.PlaneTicketRepository
            .GetAll()
            .Where(t =>
                t.Airline == request.Airline &&
                t.Plane == request.Plane &&
                t.Gate == request.Gate &&
                t.Meal == request.Meal &&
                t.LuggageKg == request.LuggageKg &&
                t.DueDate.Date == request.DueDate.Date &&
                t.FromId == request.FromId &&
                t.ToId == request.ToId &&
                !t.IsDeleted)
            .ToList();

        if (!groupTickets.Any())
            return new ResponseModel<List<UpdatePlaneTicketCommandResponse>>(null);

        var invalidStates = new[] { State.Used, State.Expired, State.Missed };

        // Tarix/saat deyisikliyi varmi?
        bool dateChanged = request.NewDueDate.HasValue && request.NewDueDate.Value != request.DueDate;
        DateTime oldDueDate = request.DueDate;
        var notifyList = new List<(string Email, string FullName, string From, string To, DateTime OldDate, DateTime NewDate)>();

        foreach (var ticket in groupTickets)
        {
            if (invalidStates.Contains(ticket.State))
                continue;

            if (ticket.State == State.Booked && request.NewState != State.Canceled)
            {
                // Tarix deyisibse Booked musteri email siyahisina elave et
                if (dateChanged && ticket.CustomerId.HasValue)
                {
                    var customer = await _unitOfWork.UserRepository.GetByIdAsync(ticket.CustomerId.Value);
                    var from = await _unitOfWork.LocationRepository.GetByIdAsync(ticket.FromId);
                    var to = await _unitOfWork.LocationRepository.GetByIdAsync(ticket.ToId);
                    if (customer != null && !string.IsNullOrEmpty(customer.Email))
                    {
                        notifyList.Add((
                            customer.Email,
                            $"{customer.Name} {customer.Surname}".Trim(),
                            from?.Name ?? "N/A",
                            to?.Name ?? "N/A",
                            oldDueDate,
                            request.NewDueDate!.Value
                        ));
                    }
                }

                ticket.Airline = request.NewAirline;
                ticket.Gate = request.NewGate;
                ticket.Meal = request.NewMeal;
                ticket.LuggageKg = request.NewLuggageKg;
                if (request.NewDueDate.HasValue)
                    ticket.DueDate = request.NewDueDate.Value;
                if (request.VariantId.HasValue)
                    ticket.VariantId = request.VariantId;
                ticket.UpdatedDate = DateTime.UtcNow;
                continue;
            }

            if (request.NewState == State.Canceled && ticket.State != State.Canceled)
            {
                if (ticket.ChosenSeatId.HasValue)
                {
                    var seat = await _unitOfWork.SeatRepository.GetByIdAsync(ticket.ChosenSeatId.Value);
                    if (seat != null)
                        seat.IsOccupied = false;
                }
                ticket.CustomerId = null;
            }

            ticket.Airline = request.NewAirline;
            ticket.Gate = request.NewGate;
            ticket.Meal = request.NewMeal;
            ticket.LuggageKg = request.NewLuggageKg;
            ticket.State = request.NewState;
            if (request.NewDueDate.HasValue)
                ticket.DueDate = request.NewDueDate.Value;
            if (request.VariantId.HasValue)
                ticket.VariantId = request.VariantId;
            ticket.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.PlaneTicketRepository.Update(ticket);
        }

        await _unitOfWork.SaveChangesAsync();

        // Email bildirisleri arxa fonda gonder
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
                            "Flight Schedule Change - StepTravel",
                            $@"
                                <p>Dear <strong>{n.FullName}</strong>,</p>
                                <p>We would like to inform you that your flight schedule has been updated.</p>
                                <br/>
                                <table style='border-collapse:collapse;'>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Route</td>
                                        <td><strong>{n.From} &rarr; {n.To}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Previous Departure</td>
                                        <td><strong style='color:#c0392b;'>{n.OldDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>New Departure</td>
                                        <td><strong style='color:#27ae60;'>{n.NewDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                                </table>
                                <br/>
                                <p style='color:#555;font-size:13px;'>Please check your updated ticket in the StepTravel app. We apologize for any inconvenience.</p>
                            "
                        );
                        _logger.LogInformation("Flight schedule change email sent to: {Email}", n.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send flight schedule change email to {Email}", n.Email);
                    }
                }
            });
        }

        var responses = groupTickets.Select(t => new UpdatePlaneTicketCommandResponse
        {
            Id = t.Id,
            Airline = t.Airline,
            Gate = t.Gate,
            Plane = t.Plane,
            Meal = t.Meal,
            LuggageKg = t.LuggageKg,
            State = t.State,
            VariantId = t.VariantId,
            DueDate = t.DueDate
        }).ToList();

        return new ResponseModel<List<UpdatePlaneTicketCommandResponse>>(responses);
    }
}