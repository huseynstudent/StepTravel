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

        var variant = request.VariantId != 0 ? await _unitOfWork.VariantRepository.GetByIdAsync(request.VariantId) : null;

        var seat = request.SeatId != 0 ? await _unitOfWork.SeatRepository.GetByIdAsync(request.SeatId) : null;
        if (seat == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);
        if (seat.IsOccupied)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        var from = request.FromId != 0 ? await _unitOfWork.LocationRepository.GetByIdAsync(request.FromId) : null;
        var to = request.ToId != 0 ? await _unitOfWork.LocationRepository.GetByIdAsync(request.ToId) : null;

        trainTicket.State = request.State;
        trainTicket.DueDate = request.DueDate;
        trainTicket.BroughtDate = DateTime.UtcNow;
        trainTicket.SeatId = request.SeatId;
        trainTicket.FromId = request.FromId;
        trainTicket.ToId = request.ToId;
        trainTicket.VariantId = request.VariantId;
        trainTicket.Seat = seat;
        trainTicket.From = from;
        trainTicket.To = to;
        trainTicket.Variant = variant;
        trainTicket.HasPet = request.HasPet;
        trainTicket.HasChild = request.HasChild;
        trainTicket.LuggageCount = request.LuggageCount;
        trainTicket.TotalLuggageKg = request.TotalLuggageKg;
        trainTicket.IsRoundTrip = request.IsRoundTrip;
        trainTicket.IsCashPayment = request.IsCashPayment;
        trainTicket.Note = request.Note;

        seat.IsOccupied = true;

        // price calculation
        var basePrice = variant?.Price ?? 0.0;
        var roundTripMultiplier = trainTicket.IsRoundTrip ? 2 : 1;
        var luggageFee = request.TotalLuggageKg > (variant?.AllowedLuggageKg ?? 0) ? 10.0 : 0.0;

        var today = DateTime.UtcNow;
        var isBirthday = user.Birthday.Month == today.Month && user.Birthday.Day == today.Day;
        var birthdayMultiplier = isBirthday ? 0.5 : 1.0;

        var manualDiscount = trainTicket.Discount is > 0 and <= 1 ? trainTicket.Discount : 1.0;

        trainTicket.Price = (basePrice * roundTripMultiplier + luggageFee) * birthdayMultiplier * manualDiscount;

        await _unitOfWork.SaveChangesAsync();

        var response = new FillTrainTicketCommandResponse
        {
            Id = trainTicket.Id,
            State = trainTicket.State,
            DueDate = trainTicket.DueDate,
            BroughtDate = trainTicket.BroughtDate,
            SeatId = trainTicket.SeatId,
            FromId = trainTicket.FromId,
            ToId = trainTicket.ToId,
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
        };

        return new ResponseModel<FillTrainTicketCommandResponse>(response);
    }
}