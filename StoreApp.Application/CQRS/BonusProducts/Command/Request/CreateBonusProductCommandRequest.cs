using MediatR;
using StoreApp.Application.CQRS.BonusProducts.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using Microsoft.AspNetCore.Http;
namespace StoreApp.Application.CQRS.BonusProducts.Command.Request
{
    public class CreateBonusProductCommandRequest
        : IRequest<ResponseModel<CreateBonusProductCommandResponse>>
    {
        public string Name { get; set; }
        public int PricePoint { get; set; }
        public int InStock { get; set; }
        public IFormFile Image { get; set; }
    }
}