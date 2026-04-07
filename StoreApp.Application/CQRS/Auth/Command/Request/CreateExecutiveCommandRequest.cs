using MediatR;
using StoreApp.Application.CQRS.Auth.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.Auth.Command.Request;

public class CreateExecutiveCommandRequest : IRequest<ResponseModel<CreateExecutiveCommandResponse>>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}