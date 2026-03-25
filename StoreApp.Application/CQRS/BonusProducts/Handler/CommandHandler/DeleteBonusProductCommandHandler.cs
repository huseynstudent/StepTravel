using MediatR;
using StoreApp.Application.CQRS.BonusProducts.Command.Request;
using StoreApp.Application.CQRS.BonusProducts.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.BonusProducts.Handler.CommandHandler
{
    class DeleteBonusProductCommandHandler
        : IRequestHandler<DeleteBonusProductCommandRequest, ResponseModel<DeleteBonusProductCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBonusProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel<DeleteBonusProductCommandResponse>> Handle(
            DeleteBonusProductCommandRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BonusProductRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync();

            var response = new DeleteBonusProductCommandResponse { IsDeleted = true };
            return new ResponseModel<DeleteBonusProductCommandResponse>(response);
        }
    }
}