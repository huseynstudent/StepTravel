namespace StoreApp.Application.CQRS.Seats.Handler.QueryHandler;

using global::StoreApp.Application.CQRS.Seats.Query.Request;
using global::StoreApp.Application.CQRS.Seats.Query.Response;
using global::StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using global::StoreApp.DAL.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetSeatsByTicketQueryHandler : IRequestHandler<GetSeatsByTicketQueryRequest, ResponseModel<List<GetSeatsByTicketQueryResponse>>>
{
    private readonly StoreAppDbContext _db;

    public GetSeatsByTicketQueryHandler(StoreAppDbContext db)
    {
        _db = db;
    }

    public async Task<ResponseModel<List<GetSeatsByTicketQueryResponse>>> Handle(GetSeatsByTicketQueryRequest request, CancellationToken cancellationToken)
    {
        List<GetSeatsByTicketQueryResponse> seats;

        if (request.TicketType.ToLower() == "plane")
        {
            // Find the group this ticket belongs to (same airline, gate, plane, dueDate, from, to)
            var anchor = await _db.PlaneTickets.FirstOrDefaultAsync(t => t.Id == request.TicketId && !t.IsDeleted, cancellationToken);
            if (anchor == null)
                return new ResponseModel<List<GetSeatsByTicketQueryResponse>>(new List<GetSeatsByTicketQueryResponse>());

            // Get all ticket IDs in the same flight group
            var groupTicketIds = await _db.PlaneTickets
                .Where(t => !t.IsDeleted
                    && t.Airline == anchor.Airline
                    && t.Gate == anchor.Gate
                    && t.Plane == anchor.Plane
                    && t.DueDate == anchor.DueDate
                    && t.FromId == anchor.FromId
                    && t.ToId == anchor.ToId)
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);

            seats = await _db.Seats
                .Include(s => s.Variant)
                .Where(s => s.PlaneTicketId != null && groupTicketIds.Contains(s.PlaneTicketId.Value) && !s.IsDeleted)
                .Select(s => new GetSeatsByTicketQueryResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsOccupied = s.IsOccupied,
                    VariantId = s.VariantId,
                    VariantName = s.Variant.Name,
                    VariantPrice = s.Variant.Price,
                    PlaneTicketId = s.PlaneTicketId
                })
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }
        else
        {
            // Train: same logic — find group by trainNumber + vagonNumber + dueDate + from + to
            var anchor = await _db.TrainTickets.FirstOrDefaultAsync(t => t.Id == request.TicketId && !t.IsDeleted, cancellationToken);
            if (anchor == null)
                return new ResponseModel<List<GetSeatsByTicketQueryResponse>>(new List<GetSeatsByTicketQueryResponse>());

            var groupTicketIds = await _db.TrainTickets
                .Where(t => !t.IsDeleted
                    && t.TrainNumber == anchor.TrainNumber
                    && t.VagonNumber == anchor.VagonNumber
                    && t.DueDate == anchor.DueDate
                    && t.FromId == anchor.FromId
                    && t.ToId == anchor.ToId)
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);

            seats = await _db.Seats
                .Include(s => s.Variant)
                .Where(s => s.TrainTicketId != null && groupTicketIds.Contains(s.TrainTicketId.Value) && !s.IsDeleted)
                .Select(s => new GetSeatsByTicketQueryResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsOccupied = s.IsOccupied,
                    VariantId = s.VariantId,
                    VariantName = s.Variant.Name,
                    VariantPrice = s.Variant.Price,
                    TrainTicketId = s.TrainTicketId
                })
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }

        return new ResponseModel<List<GetSeatsByTicketQueryResponse>>(seats);
    }
}