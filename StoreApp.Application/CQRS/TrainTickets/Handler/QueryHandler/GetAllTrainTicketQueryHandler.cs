using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.TrainTickets.Handler.QueryHandler
{
    class GetAllTrainTicketQueryHandler : IRequestHandler<GetAllTrainTicketQueryRequest, Pagination<GetAllTrainTicketQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllTrainTicketQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Pagination<GetAllTrainTicketQueryResponse>> Handle(GetAllTrainTicketQueryRequest request, CancellationToken cancellationToken)
        {
            var trainTickets = _unitOfWork.TrainTicketRepository.GetAll();
            var totalCount = trainTickets.Count();
            var paginatedTrainTickets = trainTickets.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList();

            var response = trainTickets.Select(trainTicket => new GetAllTrainTicketQueryResponse
            {
                Id = trainTicket.Id,
                TrainCompany = trainTicket.TrainCompany,
                TrainNumber = trainTicket.TrainNumber,
                VagonNumber = trainTicket.VagonNumber
            }).ToList();

            return new Pagination<GetAllTrainTicketQueryResponse>(response, totalCount, request.Page, request.Limit);
        }
    }
}