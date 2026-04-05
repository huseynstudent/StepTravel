using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

class CreateTrainTicketCommandHandler : IRequestHandler<CreateTrainTicketCommandRequest, ResponseModel<CreateTrainTicketCommandResponse>>
{

    private readonly IUnitOfWork _unitOfWork;
    public CreateTrainTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<CreateTrainTicketCommandResponse>> Handle(CreateTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var trainTicket = new TrainTicket
        {
            TrainCompany = request.TrainCompany,
            TrainNumber = request.TrainNumber,
            VagonNumber = request.VagonNumber
        };

        await _unitOfWork.TrainTicketRepository.AddAsync(trainTicket);
        await _unitOfWork.SaveChangesAsync();

        var response = new CreateTrainTicketCommandResponse
        {
            Id = trainTicket.Id,
            TrainCompany = trainTicket.TrainCompany,
            TrainNumber = trainTicket.TrainNumber,
            VagonNumber = trainTicket.VagonNumber
        };

        return new ResponseModel<CreateTrainTicketCommandResponse>(response);
    }
}