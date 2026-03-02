using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Query.Request;
using StoreApp.Application.CQRS.TrainTickets.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.TrainTickets.Handler.QueryHandler
{
    class GetTrainTicketByIdQueryHandler : IRequestHandler<GetTrainTicketByIdQueryRequest, ResponseModel<GetTrainTicketByIdQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetTrainTicketByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<GetTrainTicketByIdQueryResponse>> Handle(GetTrainTicketByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);

            if (trainTicket != null)
            {
                var response = new GetTrainTicketByIdQueryResponse
                {
                    Id = trainTicket.Id,
                    TrainCompany = trainTicket.TrainCompany,
                    TrainNumber = trainTicket.TrainNumber,
                    VagonNumber = trainTicket.VagonNumber
                };

                return new ResponseModel<GetTrainTicketByIdQueryResponse>(response);
            }

            return new ResponseModel<GetTrainTicketByIdQueryResponse>(null);
        }
    }
}