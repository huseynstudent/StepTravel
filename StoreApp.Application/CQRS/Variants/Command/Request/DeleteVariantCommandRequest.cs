using MediatR;
using StoreApp.Application.CQRS.Variants.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Variants.Command.Request
{
    public class DeleteVariantCommandRequest : IRequest<ResponseModel<DeleteVariantCommandResponse>>
    {
        public int Id { get; set; }
    }
}