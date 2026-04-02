using MediatR;
using StoreApp.Application.CQRS.User.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

namespace StoreApp.Application.CQRS.User.Command.Request;

public class ResetPasswordCommandRequest : IRequest<ResponseModel<ResetPasswordCommandResponse>>
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}