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

    public FillPlaneTicketCommandHandler(IUnitOfWork unitOfWork, StoreAppDbContext db, IConfiguration configuration, ILogger<FillPlaneTicketCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<FillPlaneTicketCommandResponse>> Handle(FillPlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var planeTicket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);
        if (planeTicket == null)
        {
            _logger.LogWarning("FillPlaneTicket failed - Ticket not found: {TicketId}", request.Id);
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);
        }

        if (planeTicket.CustomerId != null)
        {
            _logger.LogWarning("FillPlaneTicket failed - Ticket already claimed: {TicketId}", request.Id);
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("FillPlaneTicket failed - User not found: {UserId}", request.UserId);
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);
        }

        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.ChosenSeatId);
        if (seat == null || seat.IsOccupied)
        {
            _logger.LogWarning("FillPlaneTicket failed - Seat unavailable: {SeatId}", request.ChosenSeatId);
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);
        }

        if (seat.PlaneTicketId != planeTicket.Id)
        {
            _logger.LogWarning("FillPlaneTicket failed - Seat {SeatId} does not belong to ticket {TicketId}", seat.Id, planeTicket.Id);
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);
        }

        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(seat.VariantId);
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(planeTicket.FromId);
        var to = await _unitOfWork.LocationRepository.GetByIdAsync(planeTicket.ToId);

        if (from == null || to == null)
        {
            _logger.LogWarning("FillPlaneTicket failed - Location not found for ticket {TicketId}", planeTicket.Id);
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);
        }

        planeTicket.CustomerId = user.Id;
        planeTicket.Customer = user;
        planeTicket.State = State.Booked;
        planeTicket.BroughtDate = DateTime.UtcNow;
        planeTicket.ChosenSeatId = seat.Id;
        planeTicket.VariantId = seat.VariantId;
        planeTicket.Variant = variant;
        planeTicket.HasPet = request.HasPet;
        planeTicket.HasChild = request.HasChild;
        planeTicket.LuggageCount = request.LuggageCount;
        planeTicket.TotalLuggageKg = request.TotalLuggageKg;
        planeTicket.Note = request.Note;

        seat.IsOccupied = true;

        var variantAddition = variant?.Price ?? 0.0;
        double basePrice = from.CountryId == to.CountryId
            ? 100 + variantAddition
            : Math.Abs(from.DistanceToken - to.DistanceToken) * 40 + variantAddition;

        var luggageFee = request.TotalLuggageKg > (variant?.AllowedLuggageKg ?? 0) ? 10.0 : 0.0;
        var isBirthday = user.Birthday.Month == DateTime.UtcNow.Month && user.Birthday.Day == DateTime.UtcNow.Day;
        var manualDiscount = planeTicket.Discount is > 0 and <= 1 ? planeTicket.Discount : 1.0;
        planeTicket.Price = (basePrice + luggageFee) * (isBirthday ? 0.5 : 1.0) * manualDiscount;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Plane ticket {TicketId} booked by user {UserId}", planeTicket.Id, user.Id);

        _ = Task.Run(() =>
        {
            try
            {
                string sender = _configuration["EmailSettings:SenderEmail"];
                string appPass = _configuration["EmailSettings:AppPassword"];

                var emailService = new StoreApp.Application.Service.Email();
                emailService.Send(
                    sender,
                    appPass,
                    user.Email,
                    "Your Flight Ticket – StepTravel",
                    $@"
                        <p>Dear <strong>{user.Name} {user.Surname}</strong>,</p>
                        <p>Your flight ticket has been successfully booked.</p>
                        <br/>
                        <table style='border-collapse:collapse;'>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>From</td>
                                <td><strong>{from.Name}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>To</td>
                                <td><strong>{to.Name}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Seat</td>
                                <td><strong>{seat.Name}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Class</td>
                                <td><strong>{variant?.Name ?? "Standard"}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Luggage</td>
                                <td><strong>{request.LuggageCount} bag(s), {request.TotalLuggageKg} kg</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Total Price</td>
                                <td><strong>${planeTicket.Price:F2}</strong></td></tr>
                            <tr><td style='padding:4px 12px 4px 0;color:#555;'>Booked On</td>
                                <td><strong>{planeTicket.BroughtDate:dd MMM yyyy, HH:mm} UTC</strong></td></tr>
                        </table>
                        <br/>
                        <p style='color:#555;font-size:13px;'>Thank you for choosing StepTravel. Have a great flight!</p>
                    "
                );
                _logger.LogInformation("Ticket confirmation email sent to: {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send ticket confirmation email to {Email}", user.Email);
            }
        });

        return new ResponseModel<FillPlaneTicketCommandResponse>(new FillPlaneTicketCommandResponse
        {
            Id = planeTicket.Id,
            CustomerFullName = $"{user.Name} {user.Surname}", 
            CustomerEmail = user.Email,             
            State = planeTicket.State,
            DueDate = planeTicket.DueDate,                    
            BroughtDate = planeTicket.BroughtDate,
            ChosenSeatId = planeTicket.ChosenSeatId,
            VariantId = planeTicket.VariantId,
            HasPet = planeTicket.HasPet,
            HasChild = planeTicket.HasChild,
            LuggageCount = planeTicket.LuggageCount,
            TotalLuggageKg = planeTicket.TotalLuggageKg,
            Discount = planeTicket.Discount,
            Price = planeTicket.Price,
            Note = planeTicket.Note
        });
    }
}