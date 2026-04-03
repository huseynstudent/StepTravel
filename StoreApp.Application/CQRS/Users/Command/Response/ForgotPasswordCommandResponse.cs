namespace StoreApp.Application.CQRS.User.Command.Response;

public class ForgotPasswordCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}