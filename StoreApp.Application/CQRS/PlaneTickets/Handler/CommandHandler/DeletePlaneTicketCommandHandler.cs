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

    public async Task<ResponseModel<DeletePlaneTicketCommandResponse>> Handle(
        DeletePlaneTicketGroupCommandRequest request, CancellationToken cancellationToken)
    {
        var targetTicket = _unitOfWork.PlaneTicketRepository.GetAll()
            .FirstOrDefault(pt => pt.Id == request.Id && pt.CustomerId == null);

        if (targetTicket == null)
            return new ResponseModel<DeletePlaneTicketCommandResponse>(null);

        var groupTickets = _unitOfWork.PlaneTicketRepository.GetAll()
            .Where(pt =>
                pt.Airline == targetTicket.Airline &&
                pt.Plane == targetTicket.Plane &&
                pt.Gate == targetTicket.Gate &&
                pt.Meal == targetTicket.Meal &&
                pt.LuggageKg == targetTicket.LuggageKg &&
                pt.DueDate.Date == targetTicket.DueDate.Date &&
                pt.FromId == targetTicket.FromId &&
                pt.ToId == targetTicket.ToId &&
                pt.VariantId == targetTicket.VariantId &&
                pt.CustomerId == null)
            .ToList();

        var ticketIds = groupTickets.Select(t => (int?)t.Id).ToHashSet();
        var now = DateTime.UtcNow;

        var seats = _unitOfWork.SeatRepository.GetAll()
            .Where(s => ticketIds.Contains((int)s.PlaneTicketId))
            .ToList();

        foreach (var seat in seats)
        {
            seat.IsDeleted = true;
            seat.DeletedDate = now;
        }

        foreach (var ticket in groupTickets)
            await _unitOfWork.PlaneTicketRepository.DeleteAsync(ticket.Id);

        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<DeletePlaneTicketCommandResponse>(new DeletePlaneTicketCommandResponse
        {
            Id = targetTicket.Id,
            Airline = targetTicket.Airline,
            Gate = targetTicket.Gate,
            Plane = targetTicket.Plane,
            Meal = targetTicket.Meal,
            LuggageKg = targetTicket.LuggageKg
        });
    }
}