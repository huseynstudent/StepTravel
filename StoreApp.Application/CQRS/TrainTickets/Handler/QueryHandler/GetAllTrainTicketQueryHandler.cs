using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
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
            .AsQueryable();

        if (request.Date.HasValue)
            query = query.Where(tt => tt.DueDate.Date == request.Date.Value.Date);

        if (!string.IsNullOrWhiteSpace(request.TrainCompany))
            query = query.Where(tt => tt.TrainCompany.Contains(request.TrainCompany));

        if (request.FromLocationId.HasValue)
            query = query.Where(tt => tt.FromId == request.FromLocationId.Value);

        if (request.ToLocationId.HasValue)
            query = query.Where(tt => tt.ToId == request.ToLocationId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var paged = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(tt => new GetAllTrainTicketQueryResponse
            {
                Id = tt.Id,
                TrainCompany = tt.TrainCompany,
                TrainNumber = tt.TrainNumber,
                VagonNumber = tt.VagonNumber,
                DueDate = tt.DueDate,
                From = tt.From != null ? $"{tt.From.Name}, {tt.From.Country.Name}" : null,
                To = tt.To != null ? $"{tt.To.Name}, {tt.To.Country.Name}" : null
            })
            .ToListAsync(cancellationToken);

        return new Pagination<GetAllTrainTicketQueryResponse>(paged, totalCount, request.PageNumber, request.PageSize);
    }
}