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

public class FillPlaneTicketCommandHandler : IRequestHandler<FillPlaneTicketCommandRequest, ResponseModel<FillPlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FillPlaneTicketCommandHandler> _logger;

    public FillPlaneTicketCommandHandler(
        IUnitOfWork unitOfWork,
        StoreAppDbContext db,
        IConfiguration configuration,
        ILogger<FillPlaneTicketCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<FillPlaneTicketCommandResponse>> Handle(
        FillPlaneTicketCommandRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Ticket-i tap
        var ticket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);
        if (ticket == null || ticket.IsDeleted)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        // 2. Ticket artıq alınıbsa rədd et
        if (ticket.CustomerId != null || ticket.State == State.Used || ticket.State == State.Expired)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        // 3. Oturacağı tap və yoxla
        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.ChosenSeatId);
        if (seat == null || seat.IsOccupied)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        // 4. İstifadəçini tap
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        var now = DateTime.UtcNow;

        // 5. Oturacağı işğal et
        seat.IsOccupied = true;
        _unitOfWork.SeatRepository.Update(seat);

        // 6. Ticket-i doldur
        ticket.CustomerId = request.UserId;
        ticket.ChosenSeatId = request.ChosenSeatId;
        ticket.HasPet = request.HasPet;
        ticket.HasChild = request.HasChild;
        ticket.LuggageCount = request.LuggageCount;
        ticket.TotalLuggageKg = request.TotalLuggageKg;
        ticket.Note = request.Note;
        ticket.State = State.Used;
        ticket.BroughtDate = now;
        ticket.UpdatedDate = now;

        _unitOfWork.PlaneTicketRepository.Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        // 7. Response hazırla
        var response = new FillPlaneTicketCommandResponse
        {
            Id = ticket.Id,
            CustomerFullName = $"{user.Name} {user.Surname}".Trim(),
            CustomerEmail = user.Email,
            State = ticket.State,
            DueDate = ticket.DueDate,
            BroughtDate = ticket.BroughtDate,
            ChosenSeatId = ticket.ChosenSeatId,
            VariantId = ticket.VariantId,
            HasPet = ticket.HasPet,
            HasChild = ticket.HasChild,
            LuggageCount = ticket.LuggageCount,
            TotalLuggageKg = ticket.TotalLuggageKg,
            Discount = ticket.Discount,
            Price = ticket.Price,
            Note = ticket.Note,
        };

        // 8. Təsdiq e-poçtu göndər (arxa fonda)
        _ = Task.Run(() =>
        {
            try
            {
                var from = _unitOfWork.LocationRepository.GetByIdAsync(ticket.FromId).GetAwaiter().GetResult();
                var to = _unitOfWork.LocationRepository.GetByIdAsync(ticket.ToId).GetAwaiter().GetResult();
                var variant = ticket.VariantId.HasValue
                    ? _unitOfWork.VariantRepository.GetByIdAsync(ticket.VariantId.Value).GetAwaiter().GetResult()
                    : null;

                string sender = _configuration["EmailSettings:SenderEmail"];
                string appPass = _configuration["EmailSettings:AppPassword"];
                var emailService = new Service.Email();

                emailService.Send(
                    sender,
                    appPass,
                    user.Email,
                    "Your Flight Ticket Booking Confirmation – StepTravel",
                    $@"
                        <p>Dear <strong>{user.Name} {user.Surname}</strong>,</p>
                        <p>Your flight ticket has been successfully booked.</p>
                        <br/>
                        <table style='border-collapse:collapse;'>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>From</td>
                                <td><strong>{from?.Name ?? "N/A"}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>To</td>
                                <td><strong>{to?.Name ?? "N/A"}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Seat</td>
                                <td><strong>{seat.Name}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Class</td>
                                <td><strong>{variant?.Name ?? "Standard"}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Flight Date</td>
                                <td><strong>{ticket.DueDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Total Price</td>
                                <td><strong>{ticket.Price:F2} ₼</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Booked On</td>
                                <td><strong>{now:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                        </table>
                        <br/>
                        <p style='color:#555;font-size:13px;'>Thank you for choosing StepTravel. Have a great flight!</p>
                    "
                );
                _logger.LogInformation("Booking confirmation email sent to: {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send booking confirmation email to {Email}", user.Email);
            }
        });

        return new ResponseModel<FillPlaneTicketCommandResponse>(response);
    }
}