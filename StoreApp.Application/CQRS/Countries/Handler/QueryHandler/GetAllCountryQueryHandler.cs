using MediatR;
using StoreApp.Application.CQRS.Countries.Query.Request;
using StoreApp.Application.CQRS.Countries.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Countries.Handler.QueryHandler;

class GetAllCountryQueryHandler : IRequestHandler<GetAllCountryQueryRequest, Pagination<GetAllCountryQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllCountryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Pagination<GetAllCountryQueryResponse>> Handle(GetAllCountryQueryRequest request, CancellationToken cancellationToken)
    {
        var countries = _unitOfWork.CountryRepository.GetAll();
        var totalDataCount = countries.Count();
        var paginatedCountries = countries.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList();

        var response = paginatedCountries.Select(c => new GetAllCountryQueryResponse
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        return new Pagination<GetAllCountryQueryResponse>(response, totalDataCount, request.Page, request.Limit);
    }
}