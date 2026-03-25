using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface IBonusCardRepository
{
    Task AddAsync(BonusCard bounsCard);
    void Update(BonusCard bounsCard);
    Task DeleteAsync(int id);
    Task<BonusCard> GetByIdAsync(int id);
    IQueryable<BonusCard> GetAll();
}