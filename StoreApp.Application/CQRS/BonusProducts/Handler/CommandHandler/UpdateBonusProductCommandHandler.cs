using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using StoreApp.Application.CQRS.BonusProducts.Command.Request;
using StoreApp.Application.CQRS.BonusProducts.Command.Response;
using StoreApp.Application.Helpers;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.BonusProducts.Handler.CommandHandler
{
    class UpdateBonusProductCommandHandler
        : IRequestHandler<UpdateBonusProductCommandRequest, ResponseModel<UpdateBonusProductCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateBonusProductCommandHandler(
            IUnitOfWork unitOfWork,
            IHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseModel<UpdateBonusProductCommandResponse>> Handle(
            UpdateBonusProductCommandRequest request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.BonusProductRepository.GetByIdAsync(request.Id);

            string imageUrl = existing.ImageUrl;
            string imageFileName = existing.ImageFileName;

            if (request.Image != null && request.Image.Length > 0)
            {
                ImageHelper.DeleteImage(_env.ContentRootPath, existing.ImageFileName);

                var req = _httpContextAccessor.HttpContext!.Request;
                var baseUrl = $"{req.Scheme}://{req.Host}";
                (imageUrl, imageFileName) = await ImageHelper.SaveImageAsync(
                    _env.ContentRootPath, request.Image, baseUrl);
            }

            existing.Name = request.Name;
            existing.PricePoint = request.PricePoint;
            existing.InStock = request.InStock;
            existing.ImageUrl = imageUrl;
            existing.ImageFileName = imageFileName;

            _unitOfWork.BonusProductRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            var response = new UpdateBonusProductCommandResponse
            {
                Id = existing.Id,
                Name = existing.Name,
                PricePoint = existing.PricePoint,
                InStock = existing.InStock,
                ImageUrl = existing.ImageUrl
            };

            return new ResponseModel<UpdateBonusProductCommandResponse>(response);
        }
    }
}