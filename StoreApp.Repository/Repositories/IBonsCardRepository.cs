using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface IBonsCardRepository
{
    Task AddAsync(BonusCard bounsCard);
    void UpdateAsync(BonusCard bounsCard);
    void DeleteAsync(int id);
    Task<BonusCard> GetByIdAsync(int id);
    IQueryable<BonusCard> GetAll();
}