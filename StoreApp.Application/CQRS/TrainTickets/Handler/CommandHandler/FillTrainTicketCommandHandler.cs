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
        // 1. Axtarış nəticəsindən gələn bilet (qrupun nümayəndəsi)
        var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);
        if (trainTicket == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 2. İstifadəçi
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 3. Seçilmiş oturacaq
        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.ChosenSeatId);
        if (seat == null || seat.IsOccupied)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 4. Oturacağın əsl sahibi olan bileti tap
        if (seat.TrainTicketId == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        var targetTicket = await _db.TrainTickets.FirstOrDefaultAsync(
            t => t.Id == seat.TrainTicketId && !t.IsDeleted, cancellationToken);

        // 5. Oturacağın eyni qatar qrupuna aid olduğunu yoxla
        if (targetTicket == null
            || targetTicket.TrainNumber != trainTicket.TrainNumber
            || targetTicket.VagonNumber != trainTicket.VagonNumber
            || targetTicket.DueDate != trainTicket.DueDate
            || targetTicket.FromId != trainTicket.FromId
            || targetTicket.ToId != trainTicket.ToId)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 6. Hədəf bilet artıq alınıbsa rədd et
        if (targetTicket.CustomerId != null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 7. Variant, from, to
        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(seat.VariantId);
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(targetTicket.FromId);
        var to = await _unitOfWork.LocationRepository.GetByIdAsync(targetTicket.ToId);
        if (from == null || to == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 8. Bileti doldur
        targetTicket.CustomerId = user.Id;
        targetTicket.Customer = user;
        targetTicket.State = State.Booked;
        targetTicket.BroughtDate = DateTime.UtcNow;
        targetTicket.ChosenSeatId = seat.Id;
        targetTicket.VariantId = seat.VariantId;
        targetTicket.Variant = variant;
        targetTicket.HasPet = request.HasPet;
        targetTicket.HasChild = request.HasChild;
        targetTicket.LuggageCount = request.LuggageCount;
        targetTicket.TotalLuggageKg = request.TotalLuggageKg;
        targetTicket.Note = request.Note;

        seat.IsOccupied = true;

        // 9. Qiymət hesabla
        var variantAddition = variant?.Price ?? 0.0;
        double basePrice = from.CountryId == to.CountryId
            ? 70 + variantAddition
            : Math.Abs(from.DistanceToken - to.DistanceToken) * 40 + variantAddition;

        var luggageFee = request.TotalLuggageKg > (variant?.AllowedLuggageKg ?? 0) ? 10.0 : 0.0;
        var isBirthday = user.Birthday.Month == DateTime.UtcNow.Month && user.Birthday.Day == DateTime.UtcNow.Day;
        var manualDiscount = targetTicket.Discount is > 0 and <= 1 ? targetTicket.Discount : 1.0;
        targetTicket.Price = (basePrice + luggageFee) * (isBirthday ? 0.5 : 1.0) * manualDiscount;

        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<FillTrainTicketCommandResponse>(new FillTrainTicketCommandResponse
        {
            Id = targetTicket.Id,
            State = targetTicket.State,
            BroughtDate = (DateTime)targetTicket.BroughtDate,
            ChosenSeatId = (int)targetTicket.ChosenSeatId,
            VariantId = (int)targetTicket.VariantId,
            HasPet = targetTicket.HasPet,
            HasChild = targetTicket.HasChild,
            LuggageCount = targetTicket.LuggageCount,
            TotalLuggageKg = targetTicket.TotalLuggageKg,
            Discount = targetTicket.Discount,
            Price = targetTicket.Price,
            Note = targetTicket.Note
        });
    }
}