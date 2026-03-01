using StoreApp.Domain.Entities;
namespace StoreApp.Application.CQRS.Locations.Command.Response
{
    public class UpdateLocationCommandResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }
        public int CountryId { get; set; }
        public int DistanceToken { get; set; }
    }
}