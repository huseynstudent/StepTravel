using MediatR;
using StoreApp.Application.CQRS.Variants.Command.Request;
using StoreApp.Application.CQRS.Variants.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Variants.Handler.CommandHandler
{
    class UpdateVariantCommandHandler : IRequestHandler<UpdateVariantCommandRequest, ResponseModel<UpdateVariantCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateVariantCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<UpdateVariantCommandResponse>> Handle(UpdateVariantCommandRequest request, CancellationToken cancellationToken)
        {
            var variant = await _unitOfWork.VariantRepository.GetByIdAsync(request.Id);

            if (variant != null)
            {
                variant.Name = request.Name;
                variant.Price = request.Price;
                variant.AllowedLuggageKg = request.AllowedLuggageKg;
                variant.AllowedLuggageCount = request.AllowedLuggageCount;
                variant.IsPriority = request.IsPriority;

                _unitOfWork.VariantRepository.UpdateAsync(variant);
                await _unitOfWork.SaveChangesAsync();

                var response = new UpdateVariantCommandResponse
                {
                    Id = variant.Id,
                    Name = variant.Name,
                    Price = variant.Price,
                    AllowedLuggageKg = variant.AllowedLuggageKg,
                    AllowedLuggageCount = variant.AllowedLuggageCount,
                    IsPriority = variant.IsPriority
                };

                return new ResponseModel<UpdateVariantCommandResponse>(response);
            }

            return new ResponseModel<UpdateVariantCommandResponse>(null);
        }
    }
}