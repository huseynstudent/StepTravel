namespace StoreApp.Application.CQRS.Auth.Command.Response;

public class AuthResponse
{
    public int? Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}