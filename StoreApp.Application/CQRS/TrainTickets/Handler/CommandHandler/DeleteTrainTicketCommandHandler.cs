using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

public class DeleteTrainTicketCommandHandler : IRequestHandler<DeleteTrainTicketCommandRequest, ResponseModel<DeleteTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTrainTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<DeleteTrainTicketCommandResponse>> Handle(DeleteTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);
        if (trainTicket == null)
            return new ResponseModel<DeleteTrainTicketCommandResponse>(null);

        var seats = _unitOfWork.SeatRepository.GetAll()
            .Where(s => s.TrainTicketId == request.Id)
            .ToList();

        foreach (var seat in seats)
            await _unitOfWork.SeatRepository.DeleteAsync(seat.Id);

        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.TrainTicketRepository.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<DeleteTrainTicketCommandResponse>(new DeleteTrainTicketCommandResponse
        {
            Id = trainTicket.Id,
            TrainCompany = trainTicket.TrainCompany,
            TrainNumber = trainTicket.TrainNumber,
            VagonNumber = trainTicket.VagonNumber
        });
    }
}