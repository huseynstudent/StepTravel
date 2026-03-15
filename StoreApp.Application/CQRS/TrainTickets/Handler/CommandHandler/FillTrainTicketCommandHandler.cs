using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

public class FillTrainTicketCommandHandler : IRequestHandler<FillTrainTicketCommandRequest, ResponseModel<FillTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public FillTrainTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<FillTrainTicketCommandResponse>> Handle(FillTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);

        if (trainTicket == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);
        var variant = request.VariantId != 0 ? await _unitOfWork.VariantRepository.GetByIdAsync(request.VariantId) : null;
        var seat = request.SeatId != 0 ? await _unitOfWork.SeatRepository.GetByIdAsync(request.SeatId) : null;
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

        // Compute price using variant price; apply round-trip multiplier and existing discount (default 1)
        var variantPrice = variant?.Price ?? 0.0;
        var roundTripMultiplier = trainTicket.IsRoundTrip ? 2 : 1;
        trainTicket.Price = variantPrice * roundTripMultiplier * trainTicket.Discount;

        _unitOfWork.TrainTicketRepository.UpdateAsync(trainTicket);
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

