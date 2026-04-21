namespace StoreApp.Application.CQRS.Messages.Command.Response;

public class RespondToMessageCommandResponse
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Response { get; set; }
}
