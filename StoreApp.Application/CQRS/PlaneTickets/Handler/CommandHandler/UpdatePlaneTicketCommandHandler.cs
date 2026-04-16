using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;
using StoreApp.DAL.Context;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class UpdatePlaneTicketCommandHandler : IRequestHandler<UpdatePlaneTicketCommandRequest, ResponseModel<UpdatePlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdatePlaneTicketCommandHandler> _logger;

    public UpdatePlaneTicketCommandHandler(
        IUnitOfWork unitOfWork,
        StoreAppDbContext db,
        IConfiguration configuration,
        ILogger<UpdatePlaneTicketCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<UpdatePlaneTicketCommandResponse>> Handle(UpdatePlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);

        if (ticket == null)
            return new ResponseModel<UpdatePlaneTicketCommandResponse>(null);

        var invalidStates = new[] { State.Used, State.Expired, State.Missed };
        if (invalidStates.Contains(ticket.State))
            return new ResponseModel<UpdatePlaneTicketCommandResponse>(null);

        // Handle cancellation: free seat
        if (request.State == State.Canceled && ticket.State != State.Canceled)
        {
            if (ticket.ChosenSeatId.HasValue)
            {
                var seat = await _unitOfWork.SeatRepository.GetByIdAsync(ticket.ChosenSeatId.Value);
                if (seat != null)
                    seat.IsOccupied = false;
            }
            ticket.CustomerId = null;
        }

        ticket.Airline = request.Airline;
        ticket.Gate = request.Gate;
        ticket.Plane = request.Plane;
        ticket.Meal = request.Meal;
        ticket.LuggageKg = request.LuggageKg;
        ticket.State = request.State;
        if (request.VariantId.HasValue)
            ticket.VariantId = request.VariantId;
        ticket.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.PlaneTicketRepository.Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        var user = ticket.CustomerId.HasValue
            ? await _db.Users.FirstOrDefaultAsync(u => u.Id == ticket.CustomerId.Value && !u.IsDeleted, cancellationToken)
            : null;

        if (user != null)
        {
            var seat = ticket.ChosenSeatId.HasValue
                ? await _unitOfWork.SeatRepository.GetByIdAsync(ticket.ChosenSeatId.Value)
                : null;

            var variant = ticket.VariantId.HasValue
                ? await _unitOfWork.VariantRepository.GetByIdAsync(ticket.VariantId.Value)
                : null;

            var from = await _unitOfWork.LocationRepository.GetByIdAsync(ticket.FromId);
            var to = await _unitOfWork.LocationRepository.GetByIdAsync(ticket.ToId);

            _ = Task.Run(() =>
            {
                try
                {
                    string sender = _configuration["EmailSettings:SenderEmail"];
                    string appPass = _configuration["EmailSettings:AppPassword"];

                    var emailService = new Service.Email();
                    emailService.Send(
                        sender,
                        appPass,
                        user.Email,
                        "Your Flight Ticket Update – StepTravel",
                        $@"
                            <p>Dear <strong>{user.Name} {user.Surname}</strong>,</p>
                            <p>Your flight ticket has been updated.</p>
                            <br/>
                            <table style='border-collapse:collapse;'>
                                <tr><td style='padding:4px 12px 4px 0;color:#555;'>From</td>
                                    <td><strong>{from?.Name ?? "N/A"}</strong></td></tr>
                                <tr><td style='padding:4px 12px 4px 0;color:#555;'>To</td>
                                    <td><strong>{to?.Name ?? "N/A"}</strong></td></tr>
                                <tr><td style='padding:4px 12px 4px 0;color:#555;'>Seat</td>
                                    <td><strong>{seat?.Name ?? "N/A"}</strong></td></tr>
                                <tr><td style='padding:4px 12px 4px 0;color:#555;'>Class</td>
                                    <td><strong>{variant?.Name ?? "Standard"}</strong></td></tr>
                                <tr><td style='padding:4px 12px 4px 0;color:#555;'>Airline</td>
                                    <td><strong>{ticket.Airline}</strong></td></tr>
                                <tr><td style='padding:4px 12px 4px 0;color:#555;'>Gate</td>
                                    <td><strong>{ticket.Gate}</strong></td></tr>
                                <tr><td style='padding:4px 12px 4px 0;color:#555;'>Status</td>
                                    <td><strong>{ticket.State}</strong></td></tr>
                                <tr><td style='padding:4px 12px 4px 0;color:#555;'>Total Price</td>
                                    <td><strong>${ticket.Price:F2}</strong></td></tr>
                                <tr><td style='padding:4px 12px 4px 0;color:#555;'>Booked On</td>
                                    <td><strong>{ticket.BroughtDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                            </table>
                            <br/>
                            <p style='color:#555;font-size:13px;'>Thank you for choosing StepTravel. Have a great flight!</p>
                        "
                    );
                    _logger.LogInformation("Ticket update email sent to: {Email}", user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send ticket update email to {Email}", user.Email);
                }
            });
        }
        else
        {
            _logger.LogWarning("Ticket update email skipped - no customer on ticket {TicketId}", ticket.Id);
        }

        return new ResponseModel<UpdatePlaneTicketCommandResponse>(new UpdatePlaneTicketCommandResponse
        {
            Id = ticket.Id,
            Airline = ticket.Airline,
            Gate = ticket.Gate,
            Plane = ticket.Plane,
            Meal = ticket.Meal,
            LuggageKg = ticket.LuggageKg,
            State = ticket.State,
            VariantId = ticket.VariantId
        });
    }
}