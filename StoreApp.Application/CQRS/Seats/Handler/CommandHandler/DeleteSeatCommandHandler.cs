using MediatR;
using StoreApp.Application.CQRS.Seats.Command.Request;
using StoreApp.Application.CQRS.Seats.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Seats.Handler.CommandHandler;

public class DeleteSeatCommandHandler: IRequestHandler<DeleteSeatCommandRequest, ResponseModel<DeleteSeatCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteSeatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<DeleteSeatCommandResponse>> Handle(DeleteSeatCommandRequest request, CancellationToken cancellationToken)
    {
        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.Id);
        if (seat != null)
        {
            _unitOfWork.SeatRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync();
            var response = new DeleteSeatCommandResponse
            {
                Id = seat.Id,
                Name = seat.Name,
                IsOccupied = seat.IsOccupied,
                VariantId = seat.VariantId
            };
            return new ResponseModel<DeleteSeatCommandResponse>(response);
        }
        return new ResponseModel<DeleteSeatCommandResponse>(null);
    }
}
