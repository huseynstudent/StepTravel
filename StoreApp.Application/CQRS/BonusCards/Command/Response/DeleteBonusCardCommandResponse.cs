namespace StoreApp.Application.CQRS.BonusCards.Command.Response
{
    public class DeleteBonusCardCommandResponse
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public double Points { get; set; } = 0;
    }
}