using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.Locations.Query.Request;
using StoreApp.Application.CQRS.Locations.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.Locations.Handler.QueryHandler
{
    class GetAllLocationQueryHandler : IRequestHandler<GetAllLocationQueryRequest, Pagination<GetAllLocationQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllLocationQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Pagination<GetAllLocationQueryResponse>> Handle(GetAllLocationQueryRequest request, CancellationToken cancellationToken)
        {
            var locations = _unitOfWork.LocationRepository.GetAll()
                .Include(l => l.Country);

            var totalDataCount = locations.Count();
            var paginatedLocations = locations.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList();

            var response = paginatedLocations.Select(location => new GetAllLocationQueryResponse
            {
                Id = location.Id,
                Name = location.Name,
                Country = location.Country.Name,
                CountryId = location.CountryId,
                DistanceToken = location.DistanceToken
            }).ToList();

            return new Pagination<GetAllLocationQueryResponse>(response, totalDataCount, request.Page, request.Limit);
        }
    }
}