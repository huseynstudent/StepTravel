namespace StoreApp.Application.CQRS.BonusCards.Query.Response
{
    class GetByIdBonusCardQueryResponse
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public double Points { get; set; }
    }
}