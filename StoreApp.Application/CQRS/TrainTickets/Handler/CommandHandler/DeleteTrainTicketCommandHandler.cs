using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

class DeleteTrainTicketCommandHandler : IRequestHandler<DeleteTrainTicketCommandRequest, ResponseModel<DeleteTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteTrainTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<DeleteTrainTicketCommandResponse>> Handle(DeleteTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);

        if (trainTicket != null)
        {
            _unitOfWork.TrainTicketRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync();

            var response = new DeleteTrainTicketCommandResponse
            {
                Id = trainTicket.Id,
                TrainCompany = trainTicket.TrainCompany,
                TrainNumber = trainTicket.TrainNumber,
                VagonNumber = trainTicket.VagonNumber
            };

            return new ResponseModel<DeleteTrainTicketCommandResponse>(response);
        }

        return new ResponseModel<DeleteTrainTicketCommandResponse>(null);
    }
}