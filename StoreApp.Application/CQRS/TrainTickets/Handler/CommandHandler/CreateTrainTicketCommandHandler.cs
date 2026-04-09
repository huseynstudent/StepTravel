using MediatR;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

public class CreateTrainTicketCommandHandler : IRequestHandler<CreateTrainTicketCommandRequest, ResponseModel<CreateTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTrainTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<CreateTrainTicketCommandResponse>> Handle(
        CreateTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(request.FromId);
        if (from == null)
            return new ResponseModel<CreateTrainTicketCommandResponse>(null);

        var to = await _unitOfWork.LocationRepository.GetByIdAsync(request.ToId);
        if (to == null)
            return new ResponseModel<CreateTrainTicketCommandResponse>(null);

        var columns = "ABCDEFGHIJK";
        var createdTickets = new List<TrainTicket>();

        foreach (var group in request.SeatGroups)
        {
            for (int row = 1; row <= group.RowCount; row++)
            {
                for (int col = 0; col < group.SeatsPerRow; col++)
                {
                    var ticket = new TrainTicket
                    {
                        TrainCompany = request.TrainCompany,
                        TrainNumber = request.TrainNumber,
                        VagonNumber = request.VagonNumber,
                        DueDate = request.DueDate,
                        FromId = request.FromId,
                        ToId = request.ToId,
                        From = from,
                        To = to,
                        State = State.Available
                    };

                    await _unitOfWork.TrainTicketRepository.AddAsync(ticket);
                    createdTickets.Add(ticket);
                }
            }
        }
        await _unitOfWork.SaveChangesAsync();

        // Create one Seat per ticket, linked by TrainTicketId
        int ticketIndex = 0;
        foreach (var group in request.SeatGroups)
        {
            for (int row = 1; row <= group.RowCount; row++)
            {
                for (int col = 0; col < group.SeatsPerRow; col++)
                {
                    var seatName = $"{row}{columns[col]}";
                    var linkedTicket = createdTickets[ticketIndex++];

                    await _unitOfWork.SeatRepository.AddAsync(new Seat
                    {
                        Name = seatName,
                        IsOccupied = false,
                        VariantId = group.VariantId,
                        TrainTicketId = linkedTicket.Id
                    });
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();

        var first = createdTickets.First();
        return new ResponseModel<CreateTrainTicketCommandResponse>(
            new CreateTrainTicketCommandResponse
            {
                Id = first.Id,
                TrainCompany = first.TrainCompany,
                TrainNumber = first.TrainNumber,
                VagonNumber = first.VagonNumber,
                DueDate = first.DueDate,
                FromId = first.FromId,
                ToId = first.ToId,
                TotalTicketsCreated = createdTickets.Count
            });
    }
}