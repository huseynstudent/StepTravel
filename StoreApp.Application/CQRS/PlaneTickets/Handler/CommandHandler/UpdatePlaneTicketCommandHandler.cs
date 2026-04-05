using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class UpdatePlaneTicketCommandHandler : IRequestHandler<UpdatePlaneTicketCommandRequest, ResponseModel<UpdatePlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdatePlaneTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<UpdatePlaneTicketCommandResponse>> Handle(UpdatePlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var planeTicket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);
        if(planeTicket != null)
        {
            planeTicket.Airline = request.Airline;
            planeTicket.Gate = request.Gate;
            planeTicket.Plane = request.Plane;
            planeTicket.Meal = request.Meal;
            planeTicket.LuggageKg = request.LuggageKg;
            _unitOfWork.PlaneTicketRepository.Update(planeTicket);
            await _unitOfWork.SaveChangesAsync();
            var response = new UpdatePlaneTicketCommandResponse
            {
                Id = planeTicket.Id,
                Airline = planeTicket.Airline,
                Gate = planeTicket.Gate,
                Plane = planeTicket.Plane,
                Meal = planeTicket.Meal,
                LuggageKg = planeTicket.LuggageKg
            };
            return new ResponseModel<UpdatePlaneTicketCommandResponse>(response);
        }
        return new ResponseModel<UpdatePlaneTicketCommandResponse>(null);
    }
}
