using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.QueryHandler;

public class GetAllTrainTicketQueryHandler : IRequestHandler<GetAllTrainTicketQueryRequest, Pagination<GetAllTrainTicketQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTrainTicketQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Pagination<GetAllTrainTicketQueryResponse>> Handle(GetAllTrainTicketQueryRequest request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.TrainTicketRepository.GetAll()
            .Include(tt => tt.From).ThenInclude(l => l.Country)
            .Include(tt => tt.To).ThenInclude(l => l.Country)
            .Where(tt => tt.CustomerId == null && tt.State == State.Available)
            .AsQueryable();
        if (request.Date.HasValue)
            query = query.Where(tt => tt.DueDate.Date == request.Date.Value.Date);

        if (!string.IsNullOrWhiteSpace(request.TrainCompany))
            query = query.Where(tt => tt.TrainCompany.Contains(request.TrainCompany));

        if (request.FromLocationId.HasValue)
            query = query.Where(tt => tt.FromId == request.FromLocationId.Value);

        if (request.ToLocationId.HasValue)
            query = query.Where(tt => tt.ToId == request.ToLocationId.Value);

        var groupedQuery = query
            .GroupBy(tt => new { tt.TrainCompany, tt.TrainNumber, tt.VagonNumber, tt.DueDate, tt.FromId, tt.ToId })
            .Select(g => new GetAllTrainTicketQueryResponse
            {
                Id = g.First().Id,
                TrainCompany = g.Key.TrainCompany,
                TrainNumber = g.Key.TrainNumber,
                VagonNumber = g.Key.VagonNumber,
                DueDate = g.Key.DueDate,
                From = g.First().From != null ? $"{g.First().From.Name}, {g.First().From.Country.Name}" : null,
                To = g.First().To != null ? $"{g.First().To.Name}, {g.First().To.Country.Name}" : null,
                Price = g.Min(tt => tt.Price),
                AvailableSeats = g.Count()
            });

        var totalCount = await groupedQuery.CountAsync(cancellationToken);

        var paged = await groupedQuery
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new Pagination<GetAllTrainTicketQueryResponse>(paged, totalCount, request.PageNumber, request.PageSize);
    }
}