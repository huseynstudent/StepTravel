using MediatR;
using StoreApp.Application.CQRS.Auth.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Auth.Command.Request;

public class LoginUserCommandRequest : IRequest<ResponseModel<AuthResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}