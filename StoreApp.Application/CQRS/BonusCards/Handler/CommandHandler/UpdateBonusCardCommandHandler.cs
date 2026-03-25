using MediatR;
using StoreApp.Application.CQRS.BonusCards.Command.Request;
using StoreApp.Application.CQRS.BonusCards.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.BonusCards.Handler.CommandHandler
{
    class UpdateBonusCardCommandHandler : IRequestHandler<UpdateBonusCardCommandRequest, ResponseModel<UpdateBonusCardCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateBonusCardCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<UpdateBonusCardCommandResponse>> Handle(UpdateBonusCardCommandRequest request, CancellationToken cancellationToken)
        {
            var bonusCard = await _unitOfWork.BonusCardRepository.GetByIdAsync(request.Id);

            if (bonusCard != null)
            {
                bonusCard.CardNumber = request.CardNumber;
                bonusCard.Points = request.Points;

                _unitOfWork.BonusCardRepository.Update(bonusCard);
                await _unitOfWork.SaveChangesAsync();

                var response = new UpdateBonusCardCommandResponse
                {
                    Id = bonusCard.Id,
                    CardNumber = bonusCard.CardNumber,
                    Points = bonusCard.Points
                };

                return new ResponseModel<UpdateBonusCardCommandResponse>(response);
            }

            return new ResponseModel<UpdateBonusCardCommandResponse>(null);
        }
    }
}