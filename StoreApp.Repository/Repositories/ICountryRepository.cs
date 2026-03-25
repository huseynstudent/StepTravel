using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface ICountryRepository
{
    Task AddAsync(Country country);
    void Update(Country country);
    Task DeleteAsync(int id);
    Task<Country> GetByIdAsync(int id);
    IQueryable<Country> GetAll();
}