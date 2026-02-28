using MediatR;
using StoreApp.Application.CQRS.BonusCards.Query.Request;
using StoreApp.Application.CQRS.BonusCards.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.BonusCards.Handler.QueryHandler
{
    class GetAllBonusCardQueryHandler : IRequestHandler<GetAllBonusCardQueryRequest, Pagination<GetAllBonusCardQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllBonusCardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Pagination<GetAllBonusCardQueryResponse>> Handle(GetAllBonusCardQueryRequest request, CancellationToken cancellationToken)
        {
            var bonusCards = _unitOfWork.BonusCardRepository.GetAll();
            var totalDataCount = bonusCards.Count();
            var pagetedBonusCards = bonusCards.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList();

            var response = pagetedBonusCards.Select(b => new GetAllBonusCardQueryResponse
            {
                Id = b.Id,
                CardNumber = b.CardNumber,
                Points = b.Points,
            }).ToList();

            return new Pagination<GetAllBonusCardQueryResponse>(response, totalDataCount, request.Page, request.Limit);
        }
    }
}