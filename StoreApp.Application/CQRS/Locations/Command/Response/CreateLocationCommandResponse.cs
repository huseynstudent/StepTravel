using StoreApp.Domain.Entities;
namespace StoreApp.Application.CQRS.Locations.Command.Response
{
    public class CreateLocationCommandResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int CountryId { get; set; }
        public int DistanceToken { get; set; }
    }
}