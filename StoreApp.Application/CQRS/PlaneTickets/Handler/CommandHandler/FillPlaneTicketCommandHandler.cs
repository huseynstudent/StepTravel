using MediatR;
using Microsoft.EntityFrameworkCore;
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

        // Already claimed by another user
        if (planeTicket.CustomerId != null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.ChosenSeatId);
        if (seat == null || seat.IsOccupied)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        // Seat must belong to this ticket's pool
        if (seat.PlaneTicketId != planeTicket.Id)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(seat.VariantId);
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(planeTicket.FromId);
        var to = await _unitOfWork.LocationRepository.GetByIdAsync(planeTicket.ToId);
        if (from == null || to == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

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
        planeTicket.IsRoundTrip = request.IsRoundTrip;
        planeTicket.Note = request.Note;

        seat.IsOccupied = true;

        var variantAddition = variant?.Price ?? 0.0;
        double basePrice = from.CountryId == to.CountryId
            ? 100 + variantAddition
            : Math.Abs(from.DistanceToken - to.DistanceToken) * 40 + variantAddition;

        var luggageFee = request.TotalLuggageKg > (variant?.AllowedLuggageKg ?? 0) ? 10.0 : 0.0;
        var isBirthday = user.Birthday.Month == DateTime.UtcNow.Month && user.Birthday.Day == DateTime.UtcNow.Day;
        var manualDiscount = planeTicket.Discount is > 0 and <= 1 ? planeTicket.Discount : 1.0;
        planeTicket.Price = (basePrice * (planeTicket.IsRoundTrip ? 2 : 1) + luggageFee) * (isBirthday ? 0.5 : 1.0) * manualDiscount;

        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<FillPlaneTicketCommandResponse>(new FillPlaneTicketCommandResponse
        {
            Id = planeTicket.Id,
            Customer = planeTicket.Customer,
            State = planeTicket.State,
            BroughtDate = (DateTime)planeTicket.BroughtDate,
            ChosenSeatId = (int)planeTicket.ChosenSeatId,
            VariantId = (int)planeTicket.VariantId,
            HasPet = planeTicket.HasPet,
            HasChild = planeTicket.HasChild,
            LuggageCount = planeTicket.LuggageCount,
            TotalLuggageKg = planeTicket.TotalLuggageKg,
            Discount = planeTicket.Discount,
            IsRoundTrip = planeTicket.IsRoundTrip,
            Price = planeTicket.Price,
            Note = planeTicket.Note
        });
    }
}