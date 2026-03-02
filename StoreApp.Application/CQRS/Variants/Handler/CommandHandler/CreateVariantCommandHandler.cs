using MediatR;
using StoreApp.Application.CQRS.Variants.Command.Request;
using StoreApp.Application.CQRS.Variants.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Comman;
namespace StoreApp.Application.CQRS.Variants.Handler.CommandHandler
{
    class CreateVariantCommandHandler : IRequestHandler<CreateVariantCommandRequest, ResponseModel<CreateVariantCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateVariantCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<CreateVariantCommandResponse>> Handle(CreateVariantCommandRequest request, CancellationToken cancellationToken)
        {
            var variant = new Variant
            {
                Name = request.Name,
                Price = request.Price,
                AllowedLuggageKg = request.AllowedLuggageKg,
                AllowedLuggageCount = request.AllowedLuggageCount,
                IsPriority = request.IsPriority
            };

            await _unitOfWork.VariantRepository.AddAsync(variant);
            await _unitOfWork.SaveChangesAsync();

            var response = new CreateVariantCommandResponse
            {
                Id = variant.Id,
                Name = variant.Name,
                Price = variant.Price,
                AllowedLuggageKg = variant.AllowedLuggageKg,
                AllowedLuggageCount = variant.AllowedLuggageCount,
                IsPriority = variant.IsPriority
            };

            return new ResponseModel<CreateVariantCommandResponse>(response);
        }
    }
}