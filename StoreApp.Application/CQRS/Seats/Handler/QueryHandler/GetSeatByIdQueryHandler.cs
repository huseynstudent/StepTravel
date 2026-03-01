using MediatR;
using StoreApp.Application.CQRS.Seats.Query.Request;
using StoreApp.Application.CQRS.Seats.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Seats.Handler.QueryHandler;

public class GetSeatByIdQueryHandler: IRequestHandler<GetSeatByIdQueryRequest, ResponseModel<GetSeatByIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetSeatByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<GetSeatByIdQueryResponse>> Handle(GetSeatByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.Id);
        if (seat != null)
        {
            var response = new GetSeatByIdQueryResponse
            {
                Id = seat.Id,
                Name = seat.Name,
                IsOccupied = seat.IsOccupied,
                VariantId = seat.VariantId
            };
            return new ResponseModel<GetSeatByIdQueryResponse>(response);
        }
        return new ResponseModel<GetSeatByIdQueryResponse>(null);
    }
}
