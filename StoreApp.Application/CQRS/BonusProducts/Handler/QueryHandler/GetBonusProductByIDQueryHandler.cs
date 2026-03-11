using MediatR;
using StoreApp.Application.CQRS.BonusProducts.Query.Request;
using StoreApp.Application.CQRS.BonusProducts.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.BonusProducts.Handler.QueryHandler
{
    public class GetBonusProductByIdQueryHandler : IRequestHandler<GetBonusProductByIdQueryRequest, ResponseModel<GetBonusProductByIdQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBonusProductByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<GetBonusProductByIdQueryResponse>> Handle(
            GetBonusProductByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.BonusProductRepository.GetByIdAsync(request.Id);

            var response = new GetBonusProductByIdQueryResponse
            {
                Id = product.Id,
                Name = product.Name,
                PricePoint = product.PricePoint,
                InStock = product.InStock,
                ImageUrl = product.ImageUrl,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate
            };

            return new ResponseModel<GetBonusProductByIdQueryResponse>(response);
        }
    }
}