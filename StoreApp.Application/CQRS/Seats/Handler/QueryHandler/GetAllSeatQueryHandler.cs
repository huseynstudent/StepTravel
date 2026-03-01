using MediatR;
using StoreApp.Application.CQRS.Seats.Query.Request;
using StoreApp.Application.CQRS.Seats.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Seats.Handler.QueryHandler;

public class GetAllSeatQueryHandler: IRequestHandler<GetAllSeatQueryRequest, Pagination<GetAllSeatQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllSeatQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Pagination<GetAllSeatQueryResponse>> Handle(GetAllSeatQueryRequest request, CancellationToken cancellationToken)
    {
        var seats = _unitOfWork.SeatRepository.GetAll();
        var totalDataCount = seats.Count();
        var pagetedSeats = seats.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList();
        var response = pagetedSeats.Select(s => new GetAllSeatQueryResponse
        {
            Id = s.Id,
            Name = s.Name,
            IsOccupied = s.IsOccupied,
            VariantId = s.VariantId
        }).ToList();
        return new Pagination<GetAllSeatQueryResponse>(response, totalDataCount, request.Page, request.Limit);
    }
}
