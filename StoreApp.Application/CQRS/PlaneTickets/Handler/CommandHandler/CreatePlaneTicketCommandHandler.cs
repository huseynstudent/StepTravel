using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class CreatePlaneTicketCommandHandler : IRequestHandler<CreatePlaneTicketCommandRequest, ResponseModel<CreatePlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlaneTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<CreatePlaneTicketCommandResponse>> Handle(
        CreatePlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(request.FromId);
        if (from == null)
            return new ResponseModel<CreatePlaneTicketCommandResponse>(null);

        var to = await _unitOfWork.LocationRepository.GetByIdAsync(request.ToId);
        if (to == null)
            return new ResponseModel<CreatePlaneTicketCommandResponse>(null);

        var columns = "ABCDEFGHIJK";
        var createdTickets = new List<PlaneTicket>();
        int rowOffset = 0;

        // One PlaneTicket per seat derived from SeatGroups
        foreach (var group in request.SeatGroups)
        {
            // Get variant price for this group
            var variant = await _unitOfWork.VariantRepository.GetByIdAsync(group.VariantId);
            var variantPrice = variant?.Price ?? 0;

            for (int row = 1; row <= group.RowCount; row++)
            {
                for (int col = 0; col < group.SeatsPerRow; col++)
                {
                    var ticket = new PlaneTicket
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
                        To = to,
                        State = State.Pending,
                        Price = variantPrice
                    };

                    await _unitOfWork.PlaneTicketRepository.AddAsync(ticket);
                    createdTickets.Add(ticket);
                }
            }
            rowOffset += group.RowCount;
        }
        await _unitOfWork.SaveChangesAsync();

        //create one Seat per ticket, linked by PlaneTicketId
        int ticketIndex = 0;
        rowOffset = 0;
        foreach (var group in request.SeatGroups)
        {
            for (int row = 1; row <= group.RowCount; row++)
            {
                for (int col = 0; col < group.SeatsPerRow; col++)
                {
                    var seatName = $"{row + rowOffset}{columns[col]}";
                    var linkedTicket = createdTickets[ticketIndex++];

                    await _unitOfWork.SeatRepository.AddAsync(new Seat
                    {
                        Name = seatName,
                        IsOccupied = false,
                        VariantId = group.VariantId,
                        PlaneTicketId = linkedTicket.Id
                    });
                }
            }
            rowOffset += group.RowCount;
        }

        await _unitOfWork.SaveChangesAsync();

        var first = createdTickets.First();
        return new ResponseModel<CreatePlaneTicketCommandResponse>(
            new CreatePlaneTicketCommandResponse
            {
                Id = first.Id,
                Airline = first.Airline,
                Gate = first.Gate,
                Plane = first.Plane,
                Meal = first.Meal,
                LuggageKg = first.LuggageKg,
                DueDate = first.DueDate,
                FromId = first.FromId,
                ToId = first.ToId,
                State = first.State.ToString(),
                TotalTicketsCreated = createdTickets.Count
            });
    }
}