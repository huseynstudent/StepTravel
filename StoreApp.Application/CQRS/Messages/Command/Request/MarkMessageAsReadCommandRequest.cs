using MediatR;
using StoreApp.Application.CQRS.Messages.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Messages.Command.Request;

public class MarkMessageAsReadCommandRequest : IRequest<ResponseModel<MarkMessageAsReadCommandResponse>>
{
    public int Id { get; set; }
}
