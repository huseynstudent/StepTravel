using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface ILocationRepository
{
    Task AddAsync(Location location);
    void UpdateAsync(Location location);
    void DeleteAsync(int id);
    Task<Location> GetByIdAsync(int id);
    IQueryable<Location> GetAll();
}