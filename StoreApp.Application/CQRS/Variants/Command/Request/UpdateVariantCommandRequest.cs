using MediatR;
using StoreApp.Application.CQRS.Variants.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Variants.Command.Request
{
    public class UpdateVariantCommandRequest : IRequest<ResponseModel<UpdateVariantCommandResponse>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double AllowedLuggageKg { get; set; }
        public int AllowedLuggageCount { get; set; }
        public bool IsPriority { get; set; }
    }
}