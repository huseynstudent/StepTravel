using MediatR;
using StoreApp.Application.CQRS.BonusCards.Command.Request;
using StoreApp.Application.CQRS.BonusCards.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.BonusCards.Handler.CommandHandler
{
    class CreateBonusCardCommandHandler : IRequestHandler<CreateBonusCardCommandRequest, ResponseModel<CeateBonusCardCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateBonusCardCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<CeateBonusCardCommandResponse>> Handle(CreateBonusCardCommandRequest request, CancellationToken cancellationToken)
        {
            var bonusCard = new BonusCard
            {
                Points = request.Points,
                CardNumber = request.CardNumber,
            };

            await _unitOfWork.BonusCardRepository.AddAsync(bonusCard);
            await _unitOfWork.SaveChangesAsync();

            var response = new CeateBonusCardCommandResponse
            {
                Id = bonusCard.Id,
                CardNumber = bonusCard.CardNumber,
                Points = bonusCard.Points
            };

            return new ResponseModel<CeateBonusCardCommandResponse>(response);
        }
    }
}