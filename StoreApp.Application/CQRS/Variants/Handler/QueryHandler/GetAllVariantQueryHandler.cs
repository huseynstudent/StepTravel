using MediatR;
using StoreApp.Application.CQRS.Variants.Query.Request;
using StoreApp.Application.CQRS.Variants.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Variants.Handler.QueryHandler
{
    class GetAllVariantQueryHandler : IRequestHandler<GetAllVariantQueryRequest, Pagination<GetAllVariantQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllVariantQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Pagination<GetAllVariantQueryResponse>> Handle(GetAllVariantQueryRequest request, CancellationToken cancellationToken)
        {
            var variants = _unitOfWork.VariantRepository.GetAll();
            var totalDataCount = variants.Count();
            var pagedVariants = variants.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList();

            var response = pagedVariants.Select(v => new GetAllVariantQueryResponse
            {
                Id = v.Id,
                Name = v.Name,
                Price = v.Price,
                AllowedLuggageKg = v.AllowedLuggageKg,
                AllowedLuggageCount = v.AllowedLuggageCount,
                IsPriority = v.IsPriority
            }).ToList();

            return new Pagination<GetAllVariantQueryResponse>(response, totalDataCount, request.Page, request.Limit);
        }
    }
}