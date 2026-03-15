namespace StoreApp.Application.CQRS.Auth.Command.Response;
public class RegisterUserCommandResponse
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}