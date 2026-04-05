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
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(request.FromId);
        if (from == null)
            return new ResponseModel<CreateTrainTicketCommandResponse>(null);

        var to = await _unitOfWork.LocationRepository.GetByIdAsync(request.ToId);
        if (to == null)
            return new ResponseModel<CreateTrainTicketCommandResponse>(null);

        var trainTicket = new TrainTicket
        {
            TrainCompany = request.TrainCompany,
            TrainNumber = request.TrainNumber,
            VagonNumber = request.VagonNumber,
            DueDate = request.DueDate,
            FromId = request.FromId,
            ToId = request.ToId,
            From = from,
            To = to
        };

        await _unitOfWork.TrainTicketRepository.AddAsync(trainTicket);
        await _unitOfWork.SaveChangesAsync();

        foreach (var group in request.SeatGroups)
        {
            var columns = "ABCDEFGHIJK";
            for (int row = 1; row <= group.RowCount; row++)
            {
                for (int col = 0; col < group.SeatsPerRow; col++)
                {
                    await _unitOfWork.SeatRepository.AddAsync(new Seat
                    {
                        Name = $"{row}{columns[col]}",  // e.g. "1A", "1B", "3C"
                        IsOccupied = false,
                        VariantId = group.VariantId,
                        TrainTicketId = trainTicket.Id
                    });
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<CreateTrainTicketCommandResponse>(new CreateTrainTicketCommandResponse
        {
            Id = trainTicket.Id,
            TrainCompany = trainTicket.TrainCompany,
            TrainNumber = trainTicket.TrainNumber,
            VagonNumber = trainTicket.VagonNumber,
            DueDate = trainTicket.DueDate,
            FromId = trainTicket.FromId,
            ToId = trainTicket.ToId
        });
    }
}