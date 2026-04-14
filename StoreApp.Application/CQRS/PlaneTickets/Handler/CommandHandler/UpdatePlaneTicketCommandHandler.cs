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
        var ticket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);

        if (ticket == null)
            return new ResponseModel<UpdatePlaneTicketCommandResponse>(null);

        var invalidStates = new[] { State.Used, State.Expired, State.Missed };
        if (invalidStates.Contains(ticket.State))
            return new ResponseModel<UpdatePlaneTicketCommandResponse>(null);

        // Handle cancellation: free seat
        if (request.State == State.Canceled && ticket.State != State.Canceled)
        {
            if (ticket.ChosenSeatId.HasValue)
            {
                var seat = await _unitOfWork.SeatRepository.GetByIdAsync(ticket.ChosenSeatId.Value);
                if (seat != null)
                    seat.IsOccupied = false;
            }
            ticket.CustomerId = null;
        }

        ticket.Airline = request.Airline;
        ticket.Gate = request.Gate;
        ticket.Plane = request.Plane;
        ticket.Meal = request.Meal;
        ticket.LuggageKg = request.LuggageKg;
        ticket.State = request.State;
        if (request.VariantId.HasValue)
            ticket.VariantId = request.VariantId;
        ticket.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.PlaneTicketRepository.Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<UpdatePlaneTicketCommandResponse>(new UpdatePlaneTicketCommandResponse
        {
            Id = ticket.Id,
            Airline = ticket.Airline,
            Gate = ticket.Gate,
            Plane = ticket.Plane,
            Meal = ticket.Meal,
            LuggageKg = ticket.LuggageKg,
            State = ticket.State,
            VariantId = ticket.VariantId
        });
    }
}