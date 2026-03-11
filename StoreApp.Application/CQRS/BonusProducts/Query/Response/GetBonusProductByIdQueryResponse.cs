namespace StoreApp.Application.CQRS.BonusProducts.Query.Response
{
    public class GetBonusProductByIdQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PricePoint { get; set; }
        public int InStock { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}