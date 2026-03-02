using MediatR;
using StoreApp.Application.CQRS.Variants.Query.Request;
using StoreApp.Application.CQRS.Variants.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Variants.Handler.QueryHandler
{
    class GetVariantByIdQueryHandler : IRequestHandler<GetVariantByIdQueryRequest, ResponseModel<GetVariantByIdQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetVariantByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<GetVariantByIdQueryResponse>> Handle(GetVariantByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var variant = await _unitOfWork.VariantRepository.GetByIdAsync(request.Id);

            if (variant != null)
            {
                var response = new GetVariantByIdQueryResponse
                {
                    Id = variant.Id,
                    Name = variant.Name,
                    Price = variant.Price,
                    AllowedLuggageKg = variant.AllowedLuggageKg,
                    AllowedLuggageCount = variant.AllowedLuggageCount,
                    IsPriority = variant.IsPriority
                };

                return new ResponseModel<GetVariantByIdQueryResponse>(response);
            }

            return new ResponseModel<GetVariantByIdQueryResponse>(null);
        }
    }
}