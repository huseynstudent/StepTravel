using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface IBonusProductRepository
{
    Task AddAsync(BonusProduct bonusProduct);
    void Update(BonusProduct bonusProduct);
    Task DeleteAsync(int id);
    Task<BonusProduct> GetByIdAsync(int id);
    IQueryable<BonusProduct> GetAll();
}