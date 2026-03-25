using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Query.Request;
using StoreApp.Application.CQRS.PlaneTickets.Query.Response;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.QueryHandler;

public class GetPlaneTicketByIdQueryHandler:IRequestHandler<GetPlaneTicketByIdQueryRequest, GetPlaneTicketByIdQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetPlaneTicketByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<GetPlaneTicketByIdQueryResponse> Handle(GetPlaneTicketByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var planeTicket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);
        if (planeTicket == null)
        {
            return null; // exception at
        }
        return new GetPlaneTicketByIdQueryResponse
        {
            Id = planeTicket.Id,
            Airline = planeTicket.Airline,
            Gate = planeTicket.Gate,
            Plane = planeTicket.Plane,
            //Meal = planeTicket.Meal,
            HasCheckedIn = planeTicket.HasCheckedIn,
            LuggageKg = planeTicket.LuggageKg
        };
    }
}
