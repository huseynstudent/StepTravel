using MediatR;
using Microsoft.AspNetCore.Http;
using StoreApp.Application.CQRS.BonusProducts.Command.Request;
using StoreApp.Application.CQRS.BonusProducts.Command.Response;
using StoreApp.Application.Helpers;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.BonusProducts.Handler.CommandHandler
{
    public class CreateBonusProductCommandHandler
        : IRequestHandler<CreateBonusProductCommandRequest, ResponseModel<CreateBonusProductCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateBonusProductCommandHandler(IUnitOfWork unitOfWork,
            IHostEnvironment env,IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseModel<CreateBonusProductCommandResponse>> Handle(
            CreateBonusProductCommandRequest request, CancellationToken cancellationToken)
        {
            string imageUrl = null;
            string imageFileName = null;

            if (request.Image != null && request.Image.Length > 0)
            {
                var req = _httpContextAccessor.HttpContext!.Request;
                var baseUrl = $"{req.Scheme}://{req.Host}";
                (imageUrl, imageFileName) = await ImageHelper.SaveImageAsync(
                    _env.ContentRootPath, request.Image, baseUrl);
            }

            var product = new BonusProduct
            {
                Name = request.Name,
                PricePoint = request.PricePoint,
                InStock = request.InStock,
                ImageUrl = imageUrl,
                ImageFileName = imageFileName
            };

            await _unitOfWork.BonusProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var response = new CreateBonusProductCommandResponse
            {
                Id = product.Id,
                Name = product.Name,
                PricePoint = product.PricePoint,
                InStock = product.InStock,
                ImageUrl = product.ImageUrl
            };

            return new ResponseModel<CreateBonusProductCommandResponse>(response);
        }
    }
}