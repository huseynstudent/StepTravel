using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class UpdatePlaneTicketGroupCommandHandler : IRequestHandler<UpdatePlaneTicketGroupCommandRequest, ResponseModel<List<UpdatePlaneTicketCommandResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdatePlaneTicketGroupCommandHandler> _logger;

    public UpdatePlaneTicketGroupCommandHandler(
        IUnitOfWork unitOfWork,
        StoreAppDbContext db,
        IConfiguration configuration,
        ILogger<UpdatePlaneTicketGroupCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<List<UpdatePlaneTicketCommandResponse>>> Handle(UpdatePlaneTicketGroupCommandRequest request, CancellationToken cancellationToken)
    {
        // Match the group by the same key used in delete
        var tickets = _unitOfWork.PlaneTicketRepository.GetAll()
            .Where(pt =>
                pt.Airline == request.Airline &&
                pt.Plane == request.Plane &&
                pt.Gate == request.Gate &&
                pt.Meal == request.Meal &&
                pt.LuggageKg == request.LuggageKg &&
                pt.DueDate.Date == request.DueDate.Date &&
                pt.FromId == request.FromId &&
                pt.ToId == request.ToId &&
                (request.VariantId == null || pt.VariantId == request.VariantId))
            .ToList();

        if (!tickets.Any())
            return new ResponseModel<List<UpdatePlaneTicketCommandResponse>>(null);

        var invalidStates = new[] { State.Used, State.Expired, State.Missed };
        var ticketIds = tickets.Select(t => (int?)t.Id).ToHashSet();
        var now = DateTime.UtcNow;

        // Pre-fetch all seats for the group in one query
        var seats = _unitOfWork.SeatRepository.GetAll()
            .Where(s => ticketIds.Contains((int)s.PlaneTicketId))
            .ToList();

        var responses = new List<UpdatePlaneTicketCommandResponse>();
        var emailTasks = new List<(string email, string name, string surname, string fromName, string toName, string seatName, string variantName, double price, DateTime broughtDate)>();

        foreach (var ticket in tickets)
        {
            if (invalidStates.Contains(ticket.State))
                continue;

            // Handle cancellation: free seat
            if (request.NewState == State.Canceled && ticket.State != State.Canceled)
            {
                if (ticket.ChosenSeatId.HasValue)
                {
                    var seat = seats.FirstOrDefault(s => s.Id == ticket.ChosenSeatId.Value);
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
            ticket.UpdatedDate = now;

            _unitOfWork.PlaneTicketRepository.Update(ticket);

            responses.Add(new UpdatePlaneTicketCommandResponse
            {
                Id = ticket.Id,
                Airline = ticket.Airline,
                Gate = ticket.Gate,
                Plane = ticket.Plane,
                Meal = ticket.Meal,
                LuggageKg = ticket.LuggageKg,
                State = ticket.State
            });
        }

        await _unitOfWork.SaveChangesAsync();

        // Fetch email data after save — only for tickets that have a customer
        var customerTickets = tickets.Where(t => t.CustomerId.HasValue).ToList();
        if (customerTickets.Any())
        {
            var customerIds = customerTickets.Select(t => t.CustomerId.Value).Distinct().ToList();
            var users = await _db.Users
                .Where(u => customerIds.Contains(u.Id) && !u.IsDeleted)
                .ToListAsync(cancellationToken);

            var from = await _unitOfWork.LocationRepository.GetByIdAsync(request.FromId);
            var to = await _unitOfWork.LocationRepository.GetByIdAsync(request.ToId);

            foreach (var ticket in customerTickets)
            {
                var user = users.FirstOrDefault(u => u.Id == ticket.CustomerId.Value);
                if (user == null) continue;

                var seat = ticket.ChosenSeatId.HasValue
                    ? seats.FirstOrDefault(s => s.Id == ticket.ChosenSeatId.Value)
                    : null;

                var variant = ticket.VariantId.HasValue
                    ? await _unitOfWork.VariantRepository.GetByIdAsync(ticket.VariantId.Value)
                    : null;

                emailTasks.Add((
                    user.Email,
                    user.Name,
                    user.Surname,
                    from?.Name ?? "N/A",
                    to?.Name ?? "N/A",
                    seat?.Name ?? "N/A",
                    variant?.Name ?? "Standard",
                    ticket.Price,
                    ticket.BroughtDate ?? now
                ));
            }

            _ = Task.Run(() =>
            {
                string sender = _configuration["EmailSettings:SenderEmail"];
                string appPass = _configuration["EmailSettings:AppPassword"];
                var emailService = new Service.Email();

                foreach (var e in emailTasks)
                {
                    try
                    {
                        emailService.Send(
                            sender,
                            appPass,
                            e.email,
                            "Your Flight Ticket Update – StepTravel",
                            $@"
                                <p>Dear <strong>{e.name} {e.surname}</strong>,</p>
                                <p>Your flight ticket has been successfully updated.</p>
                                <br/>
                                <table style='border-collapse:collapse;'>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>From</td>
                                        <td><strong>{e.fromName}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>To</td>
                                        <td><strong>{e.toName}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Seat</td>
                                        <td><strong>{e.seatName}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Class</td>
                                        <td><strong>{e.variantName}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Airline</td>
                                        <td><strong>{request.NewAirline}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Gate</td>
                                        <td><strong>{request.NewGate}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Status</td>
                                        <td><strong>{request.NewState}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Total Price</td>
                                        <td><strong>${e.price:F2}</strong></td></tr>
                                    <tr><td style='padding:4px 12px 4px 0;color:#555;'>Booked On</td>
                                        <td><strong>{e.broughtDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                                </table>
                                <br/>
                                <p style='color:#555;font-size:13px;'>Thank you for choosing StepTravel. Have a great flight!</p>
                            "
                        );
                        _logger.LogInformation("Ticket update email sent to: {Email}", e.email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send ticket update email to {Email}", e.email);
                    }
                }
            });
        }

        return new ResponseModel<List<UpdatePlaneTicketCommandResponse>>(responses);
    }
}