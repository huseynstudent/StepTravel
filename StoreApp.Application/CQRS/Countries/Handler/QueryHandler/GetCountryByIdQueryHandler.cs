using MediatR;
using StoreApp.Application.CQRS.Countries.Query.Request;
using StoreApp.Application.CQRS.Countries.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Countries.Handler.QueryHandler
{
    class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQueryRequest, ResponseModel<GetCountryByIdQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCountryByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<GetCountryByIdQueryResponse>> Handle(GetCountryByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.CountryRepository.GetByIdAsync(request.Id);

            if (country != null)
            {
                var response = new GetCountryByIdQueryResponse
                {
                    Id = country.Id,
                    Name = country.Name
                };

                return new ResponseModel<GetCountryByIdQueryResponse>(response);
            }

            return new ResponseModel<GetCountryByIdQueryResponse>(null);

        }
    }
}