using MediatR;
using StoreApp.Application.CQRS.Messages.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Messages.Command.Request;

public class RespondToMessageCommandRequest : IRequest<ResponseModel<RespondToMessageCommandResponse>>
{
    public int Id { get; set; }
    public string Response { get; set; }
}
