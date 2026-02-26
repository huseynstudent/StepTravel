using MediatR;
using StoreApp.Application.CQRS.BonusCards.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.BonusCards.Command.Request;

public class CreateBonusCardCommandRequest : IRequest<ResponseModel<CreateBonusCardCommandResponse>>
{
    public string Name { get; set; }
}