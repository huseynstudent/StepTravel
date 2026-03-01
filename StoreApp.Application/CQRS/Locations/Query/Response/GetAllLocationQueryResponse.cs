namespace StoreApp.Application.CQRS.Locations.Query.Response
{
    class GetAllLocationQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public int CountryId { get; set; }
        public int DistanceToken { get; set; }
    }
}