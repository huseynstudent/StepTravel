namespace StoreApp.Application.CQRS.Messages.Command.Response;

public class UpdateMessageCommandResponse
{
    public int Id { get; set; }
    public string Content { get; set; }
    public bool ForAdmin { get; set; }
}
