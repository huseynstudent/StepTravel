using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.QueryHandler;

public class GetTrainTicketByDateQueryHandler : IRequestHandler<GetTrainTicketByDateQueryRequest, Pagination<GetAllTrainTicketQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetTrainTicketByDateQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Pagination<GetAllTrainTicketQueryResponse>> Handle(GetTrainTicketByDateQueryRequest request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.TrainTicketRepository.GetAll()
            .Where(tt => tt.DueDate.Date == request.Date.Date);

        var totalCount = query.Count();
        var paged = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(tt => new GetAllTrainTicketQueryResponse
            {
                Id = tt.Id,
                TrainCompany = tt.TrainCompany,
                TrainNumber = tt.TrainNumber,
                VagonNumber = tt.VagonNumber
            }).ToList();

        return new Pagination<GetAllTrainTicketQueryResponse>(paged, totalCount, request.PageNumber, request.PageSize);
    }
}
