namespace StoreApp.Application.CQRS.BonusCards.Command.Response;

public class CeateBonusCardCommandResponse
{
    public int Id { get; set; }
    public int CardNumber { get; set; }
     public double Points { get; set; } = 0;
}
