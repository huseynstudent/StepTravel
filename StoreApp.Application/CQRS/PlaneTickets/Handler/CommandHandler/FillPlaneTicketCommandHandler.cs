using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class FillPlaneTicketCommandHandler : IRequestHandler<FillPlaneTicketCommandRequest, ResponseModel<FillPlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public FillPlaneTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<FillPlaneTicketCommandResponse>> Handle(FillPlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var planeTicket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);

        if (planeTicket == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);
        var variant = request.VariantId != 0 ? await _unitOfWork.VariantRepository.GetByIdAsync(request.VariantId) : null;
        var seat = request.SeatId != 0 ? await _unitOfWork.SeatRepository.GetByIdAsync(request.SeatId) : null;
        var from = request.FromId != 0 ? await _unitOfWork.LocationRepository.GetByIdAsync(request.FromId) : null;
        var to = request.ToId != 0 ? await _unitOfWork.LocationRepository.GetByIdAsync(request.ToId) : null;

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
        planeTicket.LuggageCount = request.LuggageCount;
        planeTicket.TotalLuggageKg = request.TotalLuggageKg;
        planeTicket.IsRoundTrip = request.IsRoundTrip;
        planeTicket.IsCashPayment = request.IsCashPayment;
        planeTicket.Note = request.Note;

        // Compute price using variant price; apply round-trip multiplier and existing discount (default 1)
        var variantPrice = variant?.Price ?? 0.0;
        var roundTripMultiplier = planeTicket.IsRoundTrip ? 2 : 1;
        planeTicket.Price = variantPrice * roundTripMultiplier * planeTicket.Discount;

        _unitOfWork.PlaneTicketRepository.UpdateAsync(planeTicket);
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
