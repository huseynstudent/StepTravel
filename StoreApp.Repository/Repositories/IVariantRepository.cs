using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface IVariantRepository
{
    Task AddAsync(Variant variant);
    void Update(Variant variant);
    Task DeleteAsync(int id);
    Task<Variant> GetByIdAsync(int id);
    IQueryable<Variant> GetAll();
}