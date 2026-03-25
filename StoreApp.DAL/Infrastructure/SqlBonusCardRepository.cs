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
    public void Update(BonusCard bonusCard)
    {
        bonusCard.UpdatedDate = DateTime.UtcNow;
        _context.BonusCards.Update(bonusCard);
    }
    public async Task DeleteAsync(int id)
    {
        var bonusCard = await _context.BonusCards.FindAsync(id);

        if (bonusCard != null)
        {
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