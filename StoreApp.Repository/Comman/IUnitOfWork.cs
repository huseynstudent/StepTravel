using StoreApp.Repository.Repositories;

namespace StoreApp.Repository.Comman;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
