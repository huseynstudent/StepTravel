using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class UpdatePlaneTicketGroupCommandHandler
    : IRequestHandler<UpdatePlaneTicketGroupCommandRequest, ResponseModel<List<UpdatePlaneTicketCommandResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePlaneTicketGroupCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<List<UpdatePlaneTicketCommandResponse>>> Handle(
        UpdatePlaneTicketGroupCommandRequest request, CancellationToken cancellationToken)
    {
        var groupTickets = _unitOfWork.PlaneTicketRepository
            .GetAll()
            .Where(t =>
                t.Airline == request.Airline &&
                t.Plane == request.Plane &&
                t.Gate == request.Gate &&
                t.Meal == request.Meal &&
                t.LuggageKg == request.LuggageKg &&
                t.DueDate.Date == request.DueDate.Date &&
                t.FromId == request.FromId &&
                t.ToId == request.ToId &&
                !t.IsDeleted)
            .ToList();

        if (!groupTickets.Any())
            return new ResponseModel<List<UpdatePlaneTicketCommandResponse>>(null);

        var invalidStates = new[] { State.Used, State.Expired, State.Missed };

        foreach (var ticket in groupTickets)
        {
            if (invalidStates.Contains(ticket.State))
                continue;

            if (ticket.State == State.Booked && request.NewState != State.Canceled)
            {
                ticket.Airline = request.NewAirline;
                ticket.Gate = request.NewGate;
                ticket.Meal = request.NewMeal;
                ticket.LuggageKg = request.NewLuggageKg;
                if (request.VariantId.HasValue)
                    ticket.VariantId = request.VariantId;
                ticket.UpdatedDate = DateTime.UtcNow;
                continue;
            }

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
            if (request.VariantId.HasValue)
                ticket.VariantId = request.VariantId;
            ticket.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.PlaneTicketRepository.Update(ticket);
        }

        await _unitOfWork.SaveChangesAsync();

        var responses = groupTickets.Select(t => new UpdatePlaneTicketCommandResponse
        {
            Id = t.Id,
            Airline = t.Airline,
            Gate = t.Gate,
            Plane = t.Plane,
            Meal = t.Meal,
            LuggageKg = t.LuggageKg,
            State = t.State,
            VariantId = t.VariantId
        }).ToList();

        return new ResponseModel<List<UpdatePlaneTicketCommandResponse>>(responses);
    }
}