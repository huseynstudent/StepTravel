using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class CreatePlaneTicketCommandHandler : IRequestHandler<CreatePlaneTicketCommandRequest, ResponseModel<CreatePlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreatePlaneTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<CreatePlaneTicketCommandResponse>> Handle(CreatePlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var planeTicket = new PlaneTicket
        {
            Airline = request.Airline,
            Gate = request.Gate,
            Plane = request.Plane,
            Meal = request.Meal,
            HasCheckedIn = request.HasCheckedIn,
            LuggageKg = request.LuggageKg

        };
        await _unitOfWork.PlaneTicketRepository.AddAsync(planeTicket);
        await _unitOfWork.SaveChangesAsync();
        var response = new CreatePlaneTicketCommandResponse
        {
            Id = planeTicket.Id,
            Airline = planeTicket.Airline,
            Gate = planeTicket.Gate,
            Plane = planeTicket.Plane,
            Meal = planeTicket.Meal,
            HasCheckedIn = planeTicket.HasCheckedIn,
            LuggageKg = planeTicket.LuggageKg
        };
        return new ResponseModel<CreatePlaneTicketCommandResponse>(response);
    }
}
