using MediatR;
using StoreApp.Application.CQRS.Locations.Query.Request;
using StoreApp.Application.CQRS.Locations.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Locations.Handler.QueryHandler
{
    class GetLocationQueryByIdHandler : IRequestHandler<GetLocationByIdQueryRequest, ResponseModel<GetLocationByIdQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetLocationQueryByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<GetLocationByIdQueryResponse>> Handle(GetLocationByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var location = await _unitOfWork.LocationRepository.GetByIdAsync(request.Id);

            if (location != null)
            {
                var response = new GetLocationByIdQueryResponse
                {
                    Id = location.Id,
                    Name = location.Name,
                    Country = location.Country.Name,
                    CountryId = location.CountryId,
                    DistanceToken = location.DistanceToken
                };

                return new ResponseModel<GetLocationByIdQueryResponse>(response);
            }

            return new ResponseModel<GetLocationByIdQueryResponse>(null);
        }
    }
}