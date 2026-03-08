using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Query.Request;
using StoreApp.Application.CQRS.PlaneTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.QueryHandler;

public class GetAllPlaneTicketQueryHandler:IRequestHandler<GetAllPlaneTicketQueryRequest, Pagination<GetAllPlaneTicketQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllPlaneTicketQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Pagination<GetAllPlaneTicketQueryResponse>> Handle(GetAllPlaneTicketQueryRequest request, CancellationToken cancellationToken)
    {
        var planeTickets = _unitOfWork.PlaneTicketRepository.GetAll();
        var totalDataCount = planeTickets.Count();
        var paginatedPlaneTickets = planeTickets.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
        var response = paginatedPlaneTickets.Select(pt => new GetAllPlaneTicketQueryResponse
        {
            Id = pt.Id,
            Airline = pt.Airline,
            Gate = pt.Gate,
            Plane = pt.Plane,
            Meal = pt.Meal,
            HasCheckedIn = pt.HasCheckedIn,
            LuggageKg = pt.LuggageKg
        }).ToList();
        return new Pagination<GetAllPlaneTicketQueryResponse>(response, totalDataCount, request.PageNumber, request.PageSize);
    }
}
