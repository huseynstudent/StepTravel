using MediatR;
using StoreApp.Application.CQRS.Variants.Command.Request;
using StoreApp.Application.CQRS.Variants.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Variants.Handler.CommandHandler
{
    class DeleteVariantCommandHandler : IRequestHandler<DeleteVariantCommandRequest, ResponseModel<DeleteVariantCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteVariantCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<DeleteVariantCommandResponse>> Handle(DeleteVariantCommandRequest request, CancellationToken cancellationToken)
        {
            var variant = await _unitOfWork.VariantRepository.GetByIdAsync(request.Id);

            if (variant != null)
            {
                _unitOfWork.VariantRepository.DeleteAsync(request.Id);
                await _unitOfWork.SaveChangesAsync();

                var response = new DeleteVariantCommandResponse
                {
                    Id = variant.Id,
                    Name = variant.Name,
                    Price = variant.Price,
                    AllowedLuggageKg = variant.AllowedLuggageKg,
                    AllowedLuggageCount = variant.AllowedLuggageCount,
                    IsPriority = variant.IsPriority
                };

                return new ResponseModel<DeleteVariantCommandResponse>(response);
            }

            return new ResponseModel<DeleteVariantCommandResponse>(null);
        }
    }
}