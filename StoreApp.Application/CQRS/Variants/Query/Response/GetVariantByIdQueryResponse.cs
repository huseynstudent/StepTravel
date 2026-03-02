namespace StoreApp.Application.CQRS.Variants.Query.Response
{
    class GetVariantByIdQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double AllowedLuggageKg { get; set; }
        public int AllowedLuggageCount { get; set; }
        public bool IsPriority { get; set; }
    }
}