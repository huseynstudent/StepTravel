using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

public class UpdateTrainTicketCommandHandler : IRequestHandler<UpdateTrainTicketCommandRequest, ResponseModel<UpdateTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTrainTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<UpdateTrainTicketCommandResponse>> Handle(UpdateTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);

        if (trainTicket == null)
        {
            return new ResponseModel<UpdateTrainTicketCommandResponse>(null);
        }

        if (trainTicket.State == State.Used || trainTicket.State == State.Expired || trainTicket.State == State.Missed)
        {
            return new ResponseModel<UpdateTrainTicketCommandResponse>(null);
        }

        if (request.State == State.Canceled && trainTicket.State != State.Canceled)
        {
            if (trainTicket.ChosenSeatId.HasValue)
            {
                var seat = await _unitOfWork.SeatRepository.GetByIdAsync(trainTicket.ChosenSeatId.Value);
                if (seat != null)
                {
                    seat.IsOccupied = false;
                }
            }
            trainTicket.CustomerId = null;
        }

        trainTicket.TrainCompany = request.TrainCompany;
        trainTicket.TrainNumber = request.TrainNumber;
        trainTicket.VagonNumber = request.VagonNumber;
        trainTicket.State = request.State;
        trainTicket.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.TrainTicketRepository.Update(trainTicket);
        await _unitOfWork.SaveChangesAsync();

        var response = new UpdateTrainTicketCommandResponse
        {
            Id = trainTicket.Id,
            TrainCompany = trainTicket.TrainCompany,
            TrainNumber = trainTicket.TrainNumber,
            VagonNumber = trainTicket.VagonNumber,
            State = trainTicket.State
        };

        return new ResponseModel<UpdateTrainTicketCommandResponse>(response);
    }
}