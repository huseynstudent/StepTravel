using MediatR;
using StoreApp.Application.CQRS.BonusProducts.Query.Request;
using StoreApp.Application.CQRS.BonusProducts.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.BonusProducts.Handler.QueryHandler
{
    class GetAllBonusProductQueryHandler
        : IRequestHandler<GetAllBonusProductQueryRequest, Pagination<GetAllBonusProductQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBonusProductQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Pagination<GetAllBonusProductQueryResponse>> Handle(
            GetAllBonusProductQueryRequest request, CancellationToken cancellationToken)
        {
            var products = _unitOfWork.BonusProductRepository.GetAll();
            var totalDataCount = products.Count();

            var paginated = products
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToList();

            var response = paginated.Select(p => new GetAllBonusProductQueryResponse
            {
                Id = p.Id,
                Name = p.Name,
                PricePoint = p.PricePoint,
                InStock = p.InStock,
                ImageUrl = p.ImageUrl
            }).ToList();

            return new Pagination<GetAllBonusProductQueryResponse>(
                response, totalDataCount, request.Page, request.Limit);
        }
    }
}