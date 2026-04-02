namespace StoreApp.Application.CQRS.Users.Command.Request;

using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using MediatR;
using StoreApp.Application.CQRS.Users.Command.Response;

public class ChangePasswordCommandRequest : IRequest<ResponseModel<ChangePasswordCommandResponse>>
{
    public int UserId { get; set; }  // from JWT
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
