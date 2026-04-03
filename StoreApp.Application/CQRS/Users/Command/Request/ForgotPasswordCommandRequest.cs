using MediatR;
using StoreApp.Application.CQRS.User.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.User.Command.Request;

public class ForgotPasswordCommandRequest : IRequest<ResponseModel<ForgotPasswordCommandResponse>>
{
    public string Email { get; set; }
}