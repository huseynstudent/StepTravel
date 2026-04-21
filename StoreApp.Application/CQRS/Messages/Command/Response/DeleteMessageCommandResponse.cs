namespace StoreApp.Application.CQRS.Messages.Command.Response;

public class DeleteMessageCommandResponse
{
    public int Id { get; set; }
    public string Content { get; set; }
}
