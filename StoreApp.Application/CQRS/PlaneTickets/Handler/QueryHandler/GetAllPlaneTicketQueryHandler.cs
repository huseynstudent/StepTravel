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
            .Include(pt => pt.Variant)
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

        // In-memory grouping with correct price calculation (same formula as FillPlaneTicketCommandHandler)
        var allTickets = await query.ToListAsync(cancellationToken);

        var grouped = allTickets
            .GroupBy(pt => new { pt.Airline, pt.Plane, pt.Gate, pt.Meal, pt.LuggageKg, pt.DueDate, pt.FromId, pt.ToId })
            .Select(g =>
            {
                var first = g.First();
                var from = first.From;
                var to = first.To;

                // FillPlaneTicketCommandHandler ile eyni qiymet formulasi
                // basePrice = eyni olke: 100 + variant; ferqli olke: |distanceFrom - distanceTo| * 40 + variant
                double minPrice = g.Select(pt =>
                {
                    var variantAddition = pt.Variant?.Price ?? 0.0;
                    double basePrice = (from != null && to != null && from.CountryId == to.CountryId)
                        ? 100.0 + variantAddition
                        : Math.Abs((from?.DistanceToken ?? 0) - (to?.DistanceToken ?? 0)) * 40.0 + variantAddition;
                    return basePrice;
                }).DefaultIfEmpty(0).Min();

                return new GetAllPlaneTicketQueryResponse
                {
                    Id = first.Id,
                    Airline = g.Key.Airline,
                    Gate = g.Key.Gate,
                    Plane = g.Key.Plane,
                    Meal = g.Key.Meal,
                    LuggageKg = g.Key.LuggageKg,
                    DueDate = g.Key.DueDate,
                    From = from != null ? from.Name + ", " + from.Country?.Name : null,
                    To = to != null ? to.Name + ", " + to.Country?.Name : null,
                    Price = (decimal)minPrice,
                    AvailableSeats = g.Count(),
                    State = first.State.ToString()
                };
            })
            .ToList();

        var totalCount = grouped.Count;

        var paged = grouped
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new Pagination<GetAllPlaneTicketQueryResponse>(paged, totalCount, request.PageNumber, request.PageSize);
    }
}