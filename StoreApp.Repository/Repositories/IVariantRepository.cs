using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface IVariantRepository
{
    Task AddAsync(Variant variant);
    void UpdateAsync(Variant variant);
    void DeleteAsync(int id);
    Task<Variant> GetByIdAsync(int id);
    IQueryable<Variant> GetAll();
}