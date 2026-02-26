using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;
namespace StoreApp.DAL.Infrastructure;
public class SqlBonusCardRepository : BaseSqlRepository , IBonusCardRepository
{
    private readonly StoreAppDbContext _context;
    public SqlBonusCardRepository(StoreAppDbContext context, string connectionString) : base(connectionString)
    {
        _context = context;
    }
    public async Task AddAsync(BonusCard bonusCard)
    {
        _context.BonusCards.Add(bonusCard);
    }
    public void UpdateAsync(BonusCard bonusCard)
    {
        var existingBonusCard = _context.BonusCards.Find(bonusCard.Id);

        if (existingBonusCard != null)
        {
            existingBonusCard.CardNumber = bonusCard.CardNumber;
            existingBonusCard.Points = bonusCard.Points;
            existingBonusCard.UpdatedDate = DateTime.UtcNow;
        }
    }
    public void DeleteAsync(int id)
    {
        var bonusCard = _context.BonusCards.Find(id);

        if (bonusCard != null)
        {
            _context.BonusCards.Remove(bonusCard);
            bonusCard.IsDeleted = true;
            bonusCard.DeletedDate = DateTime.UtcNow;
        }
    }
    public IQueryable<BonusCard> GetAll()
    {
        return _context.BonusCards.Where(b => !b.IsDeleted);
    }
    public async Task<BonusCard> GetByIdAsync(int id)
    {
        return await _context.BonusCards.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
    }
}