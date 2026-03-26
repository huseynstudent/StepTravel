using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Query.Request;
using StoreApp.Application.CQRS.PlaneTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.QueryHandler;

public class GetPlaneTicketByNameQueryHandler : IRequestHandler<GetPlaneTicketByNameQueryRequest, Pagination<GetAllPlaneTicketQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetPlaneTicketByNameQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Pagination<GetAllPlaneTicketQueryResponse>> Handle(GetPlaneTicketByNameQueryRequest request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.PlaneTicketRepository.GetAll()
            .Where(pt => pt.Airline.Contains(request.Airline));

        var totalCount = query.Count();
        var paged = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(pt => new GetAllPlaneTicketQueryResponse
            {
                Id = pt.Id,
                Airline = pt.Airline,
                Gate = pt.Gate,
                Plane = pt.Plane,
                Meal = pt.Meal,
                HasCheckedIn = pt.HasCheckedIn,
                LuggageKg = pt.LuggageKg
            }).ToList();

        return new Pagination<GetAllPlaneTicketQueryResponse>(paged, totalCount, request.PageNumber, request.PageSize);
    }
}
