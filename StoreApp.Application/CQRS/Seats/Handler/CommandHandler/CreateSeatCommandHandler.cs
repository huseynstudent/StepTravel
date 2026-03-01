using StoreApp.Application.CQRS.Seats.Command.Request;
using StoreApp.Application.CQRS.Seats.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Seats.Handler.CommandHandler;

public class CreateSeatCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateSeatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<CreateSeatCommandResponse>> Handle(CreateSeatCommandRequest request, CancellationToken cancellationToken)
    {
        var seat = new Seat
        {
            Name = request.Name,
            IsOccupied = request.IsOccupied,
            VariantId = request.VariantId
        };
        await _unitOfWork.SeatRepository.AddAsync(seat);
        await _unitOfWork.SaveChangesAsync();
        var response = new CreateSeatCommandResponse
        {
            Id = seat.Id,
            Name = seat.Name,
            IsOccupied = seat.IsOccupied,
            VariantId = seat.VariantId
        };
        return new ResponseModel<CreateSeatCommandResponse>(response);
    }
}

