namespace StoreApp.Application.CQRS.Messages.Command.Response;

public class MarkMessageAsReadCommandResponse
{
    public int Id { get; set; }
    public bool HasBeenRead { get; set; }
}
