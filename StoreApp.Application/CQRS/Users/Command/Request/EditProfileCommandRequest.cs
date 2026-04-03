using MediatR;
using StoreApp.Application.CQRS.Users.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
namespace StoreApp.Application.CQRS.Users.Command.Request;

public class EditProfileCommandRequest : IRequest<ResponseModel<EditProfileCommandResponse>>
{
    // Filled from JWT in the controller — user doesn't send this
    public int UserId { get; set; }

    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateOnly Birthday { get; set; }
    public string Fin { get; set; }
}