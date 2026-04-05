using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class CreatePlaneTicketCommandHandler : IRequestHandler<CreatePlaneTicketCommandRequest, ResponseModel<CreatePlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlaneTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<CreatePlaneTicketCommandResponse>> Handle(CreatePlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(request.FromId);
        if (from == null)
            return new ResponseModel<CreatePlaneTicketCommandResponse>(null);

        var to = await _unitOfWork.LocationRepository.GetByIdAsync(request.ToId);
        if (to == null)
            return new ResponseModel<CreatePlaneTicketCommandResponse>(null);

        var planeTicket = new PlaneTicket
        {
            Airline = request.Airline,
            Gate = request.Gate,
            Plane = request.Plane,
            Meal = request.Meal,
            LuggageKg = request.LuggageKg,
            DueDate = request.DueDate,
            FromId = request.FromId,
            ToId = request.ToId,
            From = from,
            To = to
        };

        await _unitOfWork.PlaneTicketRepository.AddAsync(planeTicket);
        await _unitOfWork.SaveChangesAsync();

        int seatIndex = 1;
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
                        PlaneTicketId = planeTicket.Id
                    });
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return new ResponseModel<CreatePlaneTicketCommandResponse>(new CreatePlaneTicketCommandResponse
        {
            Id = planeTicket.Id,
            Airline = planeTicket.Airline,
            Gate = planeTicket.Gate,
            Plane = planeTicket.Plane,
            Meal = planeTicket.Meal,
            LuggageKg = planeTicket.LuggageKg,
            DueDate = planeTicket.DueDate,
            FromId = planeTicket.FromId,
            ToId = planeTicket.ToId
        });
    }
}