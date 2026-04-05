using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class DeletePlaneTicketCommandHandler : IRequestHandler<DeletePlaneTicketCommandRequest, ResponseModel<DeletePlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeletePlaneTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<DeletePlaneTicketCommandResponse>> Handle(DeletePlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var planeTicket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);
        if (planeTicket == null)
            return new ResponseModel<DeletePlaneTicketCommandResponse>(null);

        var seats = _unitOfWork.SeatRepository.GetAll()
            .Where(s => s.PlaneTicketId == request.Id)
            .ToList();
        foreach (var seat in seats)
            await _unitOfWork.SeatRepository.DeleteAsync(seat.Id);

        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.PlaneTicketRepository.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<DeletePlaneTicketCommandResponse>(new DeletePlaneTicketCommandResponse
        {
            Id = planeTicket.Id,
            Airline = planeTicket.Airline,
            Gate = planeTicket.Gate,
            Plane = planeTicket.Plane,
            Meal = planeTicket.Meal,
            LuggageKg = planeTicket.LuggageKg
        });
    }
}
