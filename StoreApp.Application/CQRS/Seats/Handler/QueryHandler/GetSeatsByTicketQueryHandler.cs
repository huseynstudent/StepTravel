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
        var query = request.TicketType.ToLower() == "plane"
            ? _db.Seats.Include(s => s.Variant).Where(s => s.PlaneTicketId == request.TicketId && !s.IsDeleted)
            : _db.Seats.Include(s => s.Variant).Where(s => s.TrainTicketId == request.TicketId && !s.IsDeleted);

        var seats = await query.Select(s => new GetSeatsByTicketQueryResponse
        {
            Id = s.Id,
            Name = s.Name,
            IsOccupied = s.IsOccupied,
            VariantId = s.VariantId,
            VariantName = s.Variant.Name,
            VariantPrice = s.Variant.Price
        }).ToListAsync(cancellationToken);

        return new ResponseModel<List<GetSeatsByTicketQueryResponse>>(seats);
    }
}