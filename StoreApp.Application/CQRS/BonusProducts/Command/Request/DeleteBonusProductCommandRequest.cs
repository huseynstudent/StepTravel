using MediatR;
using StoreApp.Application.CQRS.BonusProducts.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.BonusProducts.Command.Request
{
    public class DeleteBonusProductCommandRequest
        : IRequest<ResponseModel<DeleteBonusProductCommandResponse>>
    {
        public int Id { get; set; }
    }
}