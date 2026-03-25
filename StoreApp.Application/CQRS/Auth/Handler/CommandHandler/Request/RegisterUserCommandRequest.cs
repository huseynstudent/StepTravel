namespace StoreApp.Application.CQRS.Auth.Command.Request;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using MediatR;
using StoreApp.Application.CQRS.Auth.Command.Response;
public class RegisterUserCommandRequest : IRequest<ResponseModel<RegisterUserCommandResponse>>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateOnly Birthday { get; set; }
    public string Fin { get; set; }
    public string Conifrm { get; set; }
}