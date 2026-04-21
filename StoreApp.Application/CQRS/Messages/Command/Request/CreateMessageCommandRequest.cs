using MediatR;
using StoreApp.Application.CQRS.Messages.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Messages.Command.Request;

public class CreateMessageCommandRequest : IRequest<ResponseModel<CreateMessageCommandResponse>>
{
    public int SenderId { get; set; }
    public string Content { get; set; }
    public bool ForAdmin { get; set; }
}
