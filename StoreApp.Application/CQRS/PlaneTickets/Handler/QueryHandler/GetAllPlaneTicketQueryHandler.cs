using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.PlaneTickets.Query.Request;
using StoreApp.Application.CQRS.PlaneTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.QueryHandler;

public class GetAllPlaneTicketQueryHandler : IRequestHandler<GetAllPlaneTicketQueryRequest, Pagination<GetAllPlaneTicketQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllPlaneTicketQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Pagination<GetAllPlaneTicketQueryResponse>> Handle(GetAllPlaneTicketQueryRequest request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.PlaneTicketRepository.GetAll()
            .Include(pt => pt.From).ThenInclude(l => l.Country)
            .Include(pt => pt.To).ThenInclude(l => l.Country)
            .AsQueryable();

        if (request.Date.HasValue)
            query = query.Where(pt => pt.DueDate.Date == request.Date.Value.Date);

        if (!string.IsNullOrWhiteSpace(request.Airline))
            query = query.Where(pt => pt.Airline.Contains(request.Airline));

        if (request.FromLocationId.HasValue)
            query = query.Where(pt => pt.FromId == request.FromLocationId.Value);

        if (request.ToLocationId.HasValue)
            query = query.Where(pt => pt.ToId == request.ToLocationId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var paged = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(pt => new GetAllPlaneTicketQueryResponse
            {
                Id = pt.Id,
                Airline = pt.Airline,
                Gate = pt.Gate,
                Plane = pt.Plane,
                Meal = pt.Meal,
                LuggageKg = pt.LuggageKg,
                DueDate = pt.DueDate,
                From = pt.From != null ? $"{pt.From.Name}, {pt.From.Country.Name}" : null,
                To = pt.To != null ? $"{pt.To.Name}, {pt.To.Country.Name}" : null
            })
            .ToListAsync(cancellationToken);

        return new Pagination<GetAllPlaneTicketQueryResponse>(paged, totalCount, request.PageNumber, request.PageSize);
    }
}