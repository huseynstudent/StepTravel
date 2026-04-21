using MediatR;
using StoreApp.Application.CQRS.Messages.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Messages.Command.Request;

public class UpdateMessageCommandRequest : IRequest<ResponseModel<UpdateMessageCommandResponse>>
{
    public int Id { get; set; }
    public string Content { get; set; }
    public bool ForAdmin { get; set; }
}
