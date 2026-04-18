namespace StoreApp.Application.CQRS.Auth.Query.Response;

public class GetMeQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Fin { get; set; }
    public DateOnly Birthday { get; set; }
    public string Role { get; set; }
    public string? ProfilePicture { get; set; }
}
