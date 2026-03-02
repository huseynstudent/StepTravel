using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler
{
    class UpdateTrainTicketCommandHandler : IRequestHandler<UpdateTrainTicketCommandRequest, ResponseModel<UpdateTrainTicketCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateTrainTicketCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<UpdateTrainTicketCommandResponse>> Handle(UpdateTrainTicketCommandRequest request, CancellationToken cancellationToken)
        {
            var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);

            if(trainTicket != null)
            {
                trainTicket.TrainCompany = request.TrainCompany;
                trainTicket.TrainNumber = request.TrainNumber;
                trainTicket.VagonNumber = request.VagonNumber;

                _unitOfWork.TrainTicketRepository.UpdateAsync(trainTicket);
                await _unitOfWork.SaveChangesAsync();

                var response = new UpdateTrainTicketCommandResponse
                {
                    Id = trainTicket.Id,
                    TrainCompany = trainTicket.TrainCompany,
                    TrainNumber = trainTicket.TrainNumber,
                    VagonNumber = trainTicket.VagonNumber
                };

                return new ResponseModel<UpdateTrainTicketCommandResponse>(response);
            }

            return new ResponseModel<UpdateTrainTicketCommandResponse>(null);
        }
    }
}