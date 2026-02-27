using MediatR;
using StoreApp.Application.CQRS.BonusCards.Query.Request;
using StoreApp.Application.CQRS.BonusCards.Query.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.BonusCards.Handler.QueryHandler
{
    class GetBonusCardByIdQueryHandler : IRequestHandler<GetBonusCardByIdQueryRequest, ResponseModel<GetByIdBonusCardQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetBonusCardByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<GetByIdBonusCardQueryResponse>> Handle(GetBonusCardByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var bonusCard = await _unitOfWork.BonusCardRepository.GetByIdAsync(request.Id);

            if (bonusCard != null)
            {
                var response = new GetByIdBonusCardQueryResponse
                {
                    Id = bonusCard.Id,
                    CardNumber = bonusCard.CardNumber,
                    Points = bonusCard.Points
                };

                return new ResponseModel<GetByIdBonusCardQueryResponse>(response);
            }

            return new ResponseModel<GetByIdBonusCardQueryResponse>(null);
        }
    }
}
