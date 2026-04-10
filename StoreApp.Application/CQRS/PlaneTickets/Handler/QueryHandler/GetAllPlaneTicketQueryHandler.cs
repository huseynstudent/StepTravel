using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.PlaneTickets.Query.Request;
using StoreApp.Application.CQRS.PlaneTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
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
            .Where(pt => pt.CustomerId == null && (pt.State == State.Available || pt.State == State.Pending))
            .AsQueryable();

        if (request.Date.HasValue)
            query = query.Where(pt => pt.DueDate.Date == request.Date.Value.Date);

        if (!string.IsNullOrWhiteSpace(request.Airline))
            query = query.Where(pt => pt.Airline.Contains(request.Airline));

        if (request.FromLocationId.HasValue)
            query = query.Where(pt => pt.FromId == request.FromLocationId.Value);

        if (request.ToLocationId.HasValue)
            query = query.Where(pt => pt.ToId == request.ToLocationId.Value);

        var groupedQuery = query
            .GroupBy(pt => new { pt.Airline, pt.Plane, pt.Gate, pt.Meal, pt.LuggageKg, pt.DueDate, pt.FromId, pt.ToId })
            .Select(g => new GetAllPlaneTicketQueryResponse
            {
                Id = g.First().Id,
                Airline = g.Key.Airline,
                Gate = g.Key.Gate,
                Plane = g.Key.Plane,
                Meal = g.Key.Meal,
                LuggageKg = g.Key.LuggageKg,
                DueDate = g.Key.DueDate,
                From = g.First().From != null ? g.First().From.Name + ", " + g.First().From.Country.Name : null,
                To = g.First().To != null ? g.First().To.Name + ", " + g.First().To.Country.Name : null,
                Price = g.Min(pt => pt.Price),
                AvailableSeats = g.Count(),
                State = g.First().State.ToString()
            });

        var totalCount = await groupedQuery.CountAsync(cancellationToken);

        var paged = await groupedQuery
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new Pagination<GetAllPlaneTicketQueryResponse>(paged, totalCount, request.PageNumber, request.PageSize);
    }
}