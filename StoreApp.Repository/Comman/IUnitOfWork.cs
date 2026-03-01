using StoreApp.Repository.Repositories;

namespace StoreApp.Repository.Comman;

public interface IUnitOfWork
{
    ICountryRepository CountryRepository { get; }
    ILocationRepository LocationRepository { get; }
    ITrainTicketRepository TrainTicketRepository { get; }
    IPlaneTicketRepository PlaneTicketRepository { get; }
    IBonusCardRepository BonusCardRepository { get; }
    ISeatRepository SeatRepository { get; }
    IVariantRepository VariantRepository { get; }
    Task SaveChangesAsync();
}
