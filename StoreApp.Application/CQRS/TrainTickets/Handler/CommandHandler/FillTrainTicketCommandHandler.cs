using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

public class FillTrainTicketCommandHandler : IRequestHandler<FillTrainTicketCommandRequest, ResponseModel<FillTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreAppDbContext _db;

    public FillTrainTicketCommandHandler(IUnitOfWork unitOfWork, StoreAppDbContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public async Task<ResponseModel<FillTrainTicketCommandResponse>> Handle(FillTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);
        if (trainTicket == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // Already claimed by another user
        if (trainTicket.CustomerId != null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.ChosenSeatId);
        if (seat == null || seat.IsOccupied)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // Seat must belong to this ticket's pool
        if (seat.TrainTicketId != trainTicket.Id)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(seat.VariantId);
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(trainTicket.FromId);
        var to = await _unitOfWork.LocationRepository.GetByIdAsync(trainTicket.ToId);
        if (from == null || to == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        trainTicket.CustomerId = user.Id;
        trainTicket.Customer = user;
        trainTicket.State = State.Booked;
        trainTicket.BroughtDate = DateTime.UtcNow;
        trainTicket.ChosenSeatId = seat.Id;
        trainTicket.VariantId = seat.VariantId;
        trainTicket.Variant = variant;
        trainTicket.HasPet = request.HasPet;
        trainTicket.HasChild = request.HasChild;
        trainTicket.LuggageCount = request.LuggageCount;
        trainTicket.TotalLuggageKg = request.TotalLuggageKg;
        trainTicket.IsRoundTrip = request.IsRoundTrip;
        trainTicket.Note = request.Note;

        seat.IsOccupied = true;

        var variantAddition = variant?.Price ?? 0.0;
        double basePrice = from.CountryId == to.CountryId
            ? 70 + variantAddition
            : Math.Abs(from.DistanceToken - to.DistanceToken) * 40 + variantAddition;

        var luggageFee = request.TotalLuggageKg > (variant?.AllowedLuggageKg ?? 0) ? 10.0 : 0.0;
        var isBirthday = user.Birthday.Month == DateTime.UtcNow.Month && user.Birthday.Day == DateTime.UtcNow.Day;
        var manualDiscount = trainTicket.Discount is > 0 and <= 1 ? trainTicket.Discount : 1.0;
        trainTicket.Price = (basePrice * (trainTicket.IsRoundTrip ? 2 : 1) + luggageFee) * (isBirthday ? 0.5 : 1.0) * manualDiscount;

        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<FillTrainTicketCommandResponse>(new FillTrainTicketCommandResponse
        {
            Id = trainTicket.Id,
            State = trainTicket.State,
            BroughtDate = (DateTime)trainTicket.BroughtDate,
            ChosenSeatId = (int)trainTicket.ChosenSeatId,
            VariantId = (int)trainTicket.VariantId,
            HasPet = trainTicket.HasPet,
            HasChild = trainTicket.HasChild,
            LuggageCount = trainTicket.LuggageCount,
            TotalLuggageKg = trainTicket.TotalLuggageKg,
            Discount = trainTicket.Discount,
            IsRoundTrip = trainTicket.IsRoundTrip,
            Price = trainTicket.Price,
            Note = trainTicket.Note
        });
    }
}