using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

// Köhnə tək-bilet handler (saxlanılır)
public class DeleteTrainTicketCommandHandler : IRequestHandler<DeleteTrainTicketCommandRequest, ResponseModel<DeleteTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteTrainTicketCommandHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<ResponseModel<DeleteTrainTicketCommandResponse>> Handle(
        DeleteTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);
        if (trainTicket == null)
            return new ResponseModel<DeleteTrainTicketCommandResponse>(null);

        var seats = _unitOfWork.SeatRepository.GetAll()
            .Where(s => s.TrainTicketId == request.Id).ToList();

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

// Plane-dakı kimi: eyni qrupdakı bütün biletləri sil
public class DeleteTrainTicketGroupCommandHandler : IRequestHandler<DeleteTrainTicketGroupCommandRequest, ResponseModel<DeleteTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteTrainTicketGroupCommandHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<ResponseModel<DeleteTrainTicketCommandResponse>> Handle(
        DeleteTrainTicketGroupCommandRequest request, CancellationToken cancellationToken)
    {
        var targetTicket = _unitOfWork.TrainTicketRepository.GetAll()
            .FirstOrDefault(t => t.Id == request.Id && t.CustomerId == null);

        if (targetTicket == null)
            return new ResponseModel<DeleteTrainTicketCommandResponse>(null);

        var groupTickets = _unitOfWork.TrainTicketRepository.GetAll()
            .Where(t =>
                t.TrainCompany == targetTicket.TrainCompany &&
                t.TrainNumber == targetTicket.TrainNumber &&
                t.VagonNumber == targetTicket.VagonNumber &&
                t.DueDate.Date == targetTicket.DueDate.Date &&
                t.FromId == targetTicket.FromId &&
                t.ToId == targetTicket.ToId &&
                t.VariantId == targetTicket.VariantId)
            .ToList();

        var ticketIds = groupTickets.Select(t => (int?)t.Id).ToHashSet();
        var now = DateTime.UtcNow;

        var seats = _unitOfWork.SeatRepository.GetAll()
            .Where(s => ticketIds.Contains(s.TrainTicketId)).ToList();

        foreach (var seat in seats)
        {
            seat.IsDeleted = true;
            seat.DeletedDate = now;
        }

        foreach (var ticket in groupTickets)
            await _unitOfWork.TrainTicketRepository.DeleteAsync(ticket.Id);

        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<DeleteTrainTicketCommandResponse>(new DeleteTrainTicketCommandResponse
        {
            Id = targetTicket.Id,
            TrainCompany = targetTicket.TrainCompany,
            TrainNumber = targetTicket.TrainNumber,
            VagonNumber = targetTicket.VagonNumber
        });
    }
}