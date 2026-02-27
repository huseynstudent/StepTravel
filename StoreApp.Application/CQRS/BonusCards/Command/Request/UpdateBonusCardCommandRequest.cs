using MediatR;
using StoreApp.Application.CQRS.BonusCards.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.BonusCards.Command.Request
{
    public class UpdateBonusCardCommandRequest : IRequest<ResponseModel<UpdateBonusCardCommandResponse>>
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public double Points { get; set; }
    }
}