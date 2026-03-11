namespace StoreApp.Application.CQRS.BonusProducts.Command.Response
{
    public class UpdateBonusProductCommandResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PricePoint { get; set; }
        public int InStock { get; set; }
        public string ImageUrl { get; set; }
    }
}