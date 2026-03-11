using MediatR;
using Microsoft.AspNetCore.Http;
using StoreApp.Application.CQRS.BonusProducts.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.BonusProducts.Command.Request
{
    public class UpdateBonusProductCommandRequest
        : IRequest<ResponseModel<UpdateBonusProductCommandResponse>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PricePoint { get; set; }
        public int InStock { get; set; }
        public IFormFile Image { get; set; } // null = keep existing image
    }
}