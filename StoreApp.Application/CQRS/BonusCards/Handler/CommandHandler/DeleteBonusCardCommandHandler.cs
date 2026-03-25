using MediatR;
using StoreApp.Application.CQRS.BonusCards.Command.Request;
using StoreApp.Application.CQRS.BonusCards.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.BonusCards.Handler.CommandHandler
{
    class DeleteBonusCardCommandHandler : IRequestHandler<DeleteBonusCardCommandRequest, ResponseModel<DeleteBonusCardCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteBonusCardCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<DeleteBonusCardCommandResponse>> Handle(DeleteBonusCardCommandRequest request, CancellationToken cancellationToken)
        {
            var bonusCard = await _unitOfWork.BonusCardRepository.GetByIdAsync(request.Id);

            if (bonusCard != null)
            {
                await _unitOfWork.BonusCardRepository.DeleteAsync(request.Id);
                await _unitOfWork.SaveChangesAsync();

                var response = new DeleteBonusCardCommandResponse
                {
                    Id = bonusCard.Id,
                    CardNumber = bonusCard.CardNumber,
                    Points = bonusCard.Points
                };

                return new ResponseModel<DeleteBonusCardCommandResponse>(response);
            }

            return new ResponseModel<DeleteBonusCardCommandResponse>(null);
        }
    }
}