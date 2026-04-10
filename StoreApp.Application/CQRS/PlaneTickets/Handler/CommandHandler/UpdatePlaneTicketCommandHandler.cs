using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
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

        if (planeTicket == null)
        {
            return new ResponseModel<UpdatePlaneTicketCommandResponse>(null);
        }

        if (planeTicket.State == State.Used || planeTicket.State == State.Expired || planeTicket.State == State.Missed)
        {
            return new ResponseModel<UpdatePlaneTicketCommandResponse>(null);
        }

        if (request.State == State.Canceled && planeTicket.State != State.Canceled)
        {
            if (planeTicket.ChosenSeatId.HasValue)
            {
                var seat = await _unitOfWork.SeatRepository.GetByIdAsync(planeTicket.ChosenSeatId.Value);
                if (seat != null)
                {
                    seat.IsOccupied = false;
                }
            }
            planeTicket.CustomerId = null;
        }

        planeTicket.Airline = request.Airline;
        planeTicket.Gate = request.Gate;
        planeTicket.Plane = request.Plane;
        planeTicket.Meal = request.Meal;
        planeTicket.LuggageKg = request.LuggageKg;
        planeTicket.State = request.State;
        planeTicket.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.PlaneTicketRepository.Update(planeTicket);
        await _unitOfWork.SaveChangesAsync();

        var response = new UpdatePlaneTicketCommandResponse
        {
            Id = planeTicket.Id,
            Airline = planeTicket.Airline,
            Gate = planeTicket.Gate,
            Plane = planeTicket.Plane,
            Meal = planeTicket.Meal,
            LuggageKg = planeTicket.LuggageKg,
            State = planeTicket.State
        };

        return new ResponseModel<UpdatePlaneTicketCommandResponse>(response);
    }
}