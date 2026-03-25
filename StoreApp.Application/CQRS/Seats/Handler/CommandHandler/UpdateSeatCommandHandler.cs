using MediatR;
using StoreApp.Application.CQRS.Seats.Command.Request;
using StoreApp.Application.CQRS.Seats.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Seats.Handler.CommandHandler;

class UpdateSeatCommandHandler : IRequestHandler<UpdateSeatCommandRequest, ResponseModel<UpdateSeatCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateSeatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<UpdateSeatCommandResponse>> Handle(UpdateSeatCommandRequest request, CancellationToken cancellationToken)
    {
        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.Id);
        if (seat != null)
        {
            seat.Name = request.Name;
            seat.IsOccupied = request.IsOccupied;
            seat.VariantId = request.VariantId;
            _unitOfWork.SeatRepository.Update(seat);
            await _unitOfWork.SaveChangesAsync();
            var response = new UpdateSeatCommandResponse
            {
                Id = seat.Id,
                Name = seat.Name,
                IsOccupied = seat.IsOccupied,
                VariantId = seat.VariantId
            };
            return new ResponseModel<UpdateSeatCommandResponse>(response);
        }
        return new ResponseModel<UpdateSeatCommandResponse>(null);
    }
}
