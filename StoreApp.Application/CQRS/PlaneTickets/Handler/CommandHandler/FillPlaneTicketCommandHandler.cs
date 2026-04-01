using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class FillPlaneTicketCommandHandler : IRequestHandler<FillPlaneTicketCommandRequest, ResponseModel<FillPlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreAppDbContext _db;

    public FillPlaneTicketCommandHandler(IUnitOfWork unitOfWork, StoreAppDbContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public async Task<ResponseModel<FillPlaneTicketCommandResponse>> Handle(FillPlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var planeTicket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);
        if (planeTicket == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        var variant = request.VariantId != 0 ? await _unitOfWork.VariantRepository.GetByIdAsync(request.VariantId) : null;

        var seat = request.SeatId != 0 ? await _unitOfWork.SeatRepository.GetByIdAsync(request.SeatId) : null;
        if (seat == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);
        if (seat.IsOccupied)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        var from = request.FromId != 0 ? await _unitOfWork.LocationRepository.GetByIdAsync(request.FromId) : null;
        if (from == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        var to = request.ToId != 0 ? await _unitOfWork.LocationRepository.GetByIdAsync(request.ToId) : null;
        if (to == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        planeTicket.State = request.State;
        planeTicket.DueDate = request.DueDate;
        planeTicket.BroughtDate = DateTime.UtcNow;
        planeTicket.SeatId = request.SeatId;
        planeTicket.FromId = request.FromId;
        planeTicket.ToId = request.ToId;
        planeTicket.VariantId = request.VariantId;
        planeTicket.Seat = seat;
        planeTicket.From = from;
        planeTicket.To = to;
        planeTicket.Variant = variant;
        planeTicket.HasPet = request.HasPet;
        planeTicket.HasChild = request.HasChild;
        planeTicket.HasCheckedIn= request.HasCheckedIn;
        planeTicket.LuggageCount = request.LuggageCount;
        planeTicket.TotalLuggageKg = request.TotalLuggageKg;
        planeTicket.IsRoundTrip = request.IsRoundTrip;
        planeTicket.IsCashPayment = request.IsCashPayment;
        planeTicket.Note = request.Note;

        seat.IsOccupied = true;

        // base price from distance token logic
        var variantAddition = variant?.Price ?? 0.0;
        double basePrice;

        if (from.CountryId == to.CountryId)
        {
            basePrice = 150 + variantAddition;
        }
        else
        {
            var distance = Math.Abs(from.DistanceToken - to.DistanceToken);
            basePrice = distance * 40 + variantAddition;
        }

        var roundTripMultiplier = planeTicket.IsRoundTrip ? 2 : 1;
        var luggageFee = request.TotalLuggageKg > (variant?.AllowedLuggageKg ?? 0) ? 10.0 : 0.0;
        var today = DateTime.UtcNow;
        var isBirthday = user.Birthday.Month == today.Month && user.Birthday.Day == today.Day;
        var birthdayMultiplier = isBirthday ? 0.5 : 1.0;
        var manualDiscount = planeTicket.Discount is > 0 and <= 1 ? planeTicket.Discount : 1.0;
        planeTicket.Price = (basePrice * roundTripMultiplier + luggageFee) * birthdayMultiplier * manualDiscount;

        await _unitOfWork.SaveChangesAsync();

        var response = new FillPlaneTicketCommandResponse
        {
            Id = planeTicket.Id,
            State = planeTicket.State,
            DueDate = planeTicket.DueDate,
            BroughtDate = planeTicket.BroughtDate,
            SeatId = planeTicket.SeatId,
            FromId = planeTicket.FromId,
            ToId = planeTicket.ToId,
            VariantId = planeTicket.VariantId,
            HasPet = planeTicket.HasPet,
            HasChild = planeTicket.HasChild,
            HasCheckedIn = true,
            LuggageCount = planeTicket.LuggageCount,
            TotalLuggageKg = planeTicket.TotalLuggageKg,
            Discount = planeTicket.Discount,
            IsRoundTrip = planeTicket.IsRoundTrip,
            IsCashPayment = planeTicket.IsCashPayment,
            Price = planeTicket.Price,
            Note = planeTicket.Note
        };

        return new ResponseModel<FillPlaneTicketCommandResponse>(response);
    }
}