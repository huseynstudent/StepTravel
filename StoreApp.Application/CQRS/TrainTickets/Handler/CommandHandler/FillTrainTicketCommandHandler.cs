using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
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

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        var seat = request.ChosenSeatId != 0 ? await _unitOfWork.SeatRepository.GetByIdAsync(request.ChosenSeatId) : null;
        if (seat == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);
        if (seat.IsOccupied)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);
        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(seat.VariantId);

        var from = await _unitOfWork.LocationRepository.GetByIdAsync(trainTicket.FromId);
        var to = await _unitOfWork.LocationRepository.GetByIdAsync(trainTicket.ToId);
        if (from == null || to == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        trainTicket.Customer = user;
        trainTicket.State = request.State;
        trainTicket.BroughtDate = DateTime.UtcNow;
        trainTicket.ChosenSeatId = seat.Id;
        trainTicket.VariantId = seat.VariantId;
        trainTicket.Variant = variant;
        trainTicket.HasPet = request.HasPet;
        trainTicket.HasChild = request.HasChild;
        trainTicket.LuggageCount = request.LuggageCount;
        trainTicket.TotalLuggageKg = request.TotalLuggageKg;
        trainTicket.IsRoundTrip = request.IsRoundTrip;
        trainTicket.IsCashPayment = request.IsCashPayment;
        trainTicket.Note = request.Note;

        seat.IsOccupied = true;

        var variantAddition = variant?.Price ?? 0.0;
        double basePrice;

        if (from.CountryId == to.CountryId)
        {
            basePrice = 70 + variantAddition;
        }
        else
        {
            var distance = Math.Abs(from.DistanceToken - to.DistanceToken);
            basePrice = distance * 40 + variantAddition;
        }

        var roundTripMultiplier = trainTicket.IsRoundTrip ? 2 : 1;
        var luggageFee = request.TotalLuggageKg > (variant?.AllowedLuggageKg ?? 0) ? 10.0 : 0.0;
        var today = DateTime.UtcNow;
        var isBirthday = user.Birthday.Month == today.Month && user.Birthday.Day == today.Day;
        var birthdayMultiplier = isBirthday ? 0.5 : 1.0;
        var manualDiscount = trainTicket.Discount is > 0 and <= 1 ? trainTicket.Discount : 1.0;
        trainTicket.Price = (basePrice * roundTripMultiplier + luggageFee) * birthdayMultiplier * manualDiscount;

        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<FillTrainTicketCommandResponse>(new FillTrainTicketCommandResponse
        {
            Id = trainTicket.Id,
            State = trainTicket.State,
            BroughtDate = trainTicket.BroughtDate,
            ChosenSeatId = trainTicket.ChosenSeatId,
            VariantId = trainTicket.VariantId,
            HasPet = trainTicket.HasPet,
            HasChild = trainTicket.HasChild,
            LuggageCount = trainTicket.LuggageCount,
            TotalLuggageKg = trainTicket.TotalLuggageKg,
            Discount = trainTicket.Discount,
            IsRoundTrip = trainTicket.IsRoundTrip,
            IsCashPayment = trainTicket.IsCashPayment,
            Price = trainTicket.Price,
            Note = trainTicket.Note
        });
    }
}