using MediatR;
using StoreApp.Application.CQRS.BonusCards.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.BonusCards.Command.Request
{
    public class DeleteBonusCardRequest : IRequest<ResponseModel<DeleteBonusCardCommandResponse>>
    {
        public int Id { get; set; }
    }
}