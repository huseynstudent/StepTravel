namespace StoreApp.Application.CQRS.Users.Command.Response;

public class ChangePasswordCommandResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}
