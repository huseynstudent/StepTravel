using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

public class UpdatePlaneTicketGroupCommandHandler : IRequestHandler<UpdatePlaneTicketGroupCommandRequest, ResponseModel<List<UpdatePlaneTicketCommandResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePlaneTicketGroupCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<List<UpdatePlaneTicketCommandResponse>>> Handle(UpdatePlaneTicketGroupCommandRequest request, CancellationToken cancellationToken)
    {
        var tickets = _unitOfWork.PlaneTicketRepository.GetAll()
            .Where(pt =>
                pt.Airline == request.Airline &&
                pt.Plane == request.Plane &&
                pt.Gate == request.Gate &&
                pt.Meal == request.Meal &&
                pt.LuggageKg == request.LuggageKg &&
                pt.DueDate.Date == request.DueDate.Date &&
                pt.FromId == request.FromId &&
                pt.ToId == request.ToId &&
                (request.VariantId == null || pt.VariantId == request.VariantId) &&
                pt.CustomerId == null)
            .ToList();

        if (!tickets.Any())
            return new ResponseModel<List<UpdatePlaneTicketCommandResponse>>(null);

        // Guard: skip groups that are in a terminal state
        var invalidStates = new[] { State.Used, State.Expired, State.Missed };
        tickets = tickets.Where(pt => !invalidStates.Contains(pt.State)).ToList();

        if (!tickets.Any())
            return new ResponseModel<List<UpdatePlaneTicketCommandResponse>>(null);

        var now = DateTime.UtcNow;
        var responses = new List<UpdatePlaneTicketCommandResponse>();

        foreach (var ticket in tickets)
        {
            // Handle cancellation: free seats
            if (request.NewState == State.Canceled && ticket.State != State.Canceled)
            {
                if (ticket.ChosenSeatId.HasValue)
                {
                    var seat = await _unitOfWork.SeatRepository.GetByIdAsync(ticket.ChosenSeatId.Value);
                    if (seat != null)
                        seat.IsOccupied = false;
                }
                ticket.CustomerId = null;
            }

            ticket.Airline = request.NewAirline;
            ticket.Gate = request.NewGate;
            ticket.Meal = request.NewMeal;
            ticket.LuggageKg = request.NewLuggageKg;
            ticket.State = request.NewState;
            ticket.UpdatedDate = now;

            _unitOfWork.PlaneTicketRepository.Update(ticket);

            responses.Add(new UpdatePlaneTicketCommandResponse
            {
                Id = ticket.Id,
                Airline = ticket.Airline,
                Gate = ticket.Gate,
                Plane = ticket.Plane,
                Meal = ticket.Meal,
                LuggageKg = ticket.LuggageKg,
                State = ticket.State
            });
        }

        await _unitOfWork.SaveChangesAsync();
        return new ResponseModel<List<UpdatePlaneTicketCommandResponse>>(responses);
    }
}