using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

public class DeletePlaneTicketGroupCommandHandler : IRequestHandler<DeletePlaneTicketGroupCommandRequest, ResponseModel<DeletePlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePlaneTicketGroupCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<DeletePlaneTicketCommandResponse>> Handle(DeletePlaneTicketGroupCommandRequest request, CancellationToken cancellationToken)
    {
        // Match the entire group by the same key used in GetAll grouping
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
            return new ResponseModel<DeletePlaneTicketCommandResponse>(null);

        var first = tickets.First();
        var ticketIds = tickets.Select(t => (int?)t.Id).ToHashSet();
        var now = DateTime.UtcNow;

        // Soft-delete all seats belonging to any ticket in the group
        var seats = _unitOfWork.SeatRepository.GetAll()
            .Where(s => ticketIds.Contains(s.PlaneTicketId))
            .ToList();

        foreach (var seat in seats)
        {
            seat.IsDeleted = true;
            seat.DeletedDate = now;
        }

        // Delete all tickets in the group
        foreach (var ticket in tickets)
            await _unitOfWork.PlaneTicketRepository.DeleteAsync(ticket.Id);

        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<DeletePlaneTicketCommandResponse>(new DeletePlaneTicketCommandResponse
        {
            Id = first.Id,
            Airline = first.Airline,
            Gate = first.Gate,
            Plane = first.Plane,
            Meal = first.Meal,
            LuggageKg = first.LuggageKg
        });
    }
}