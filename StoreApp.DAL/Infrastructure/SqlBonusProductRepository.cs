using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;

namespace StoreApp.DAL.Infrastructure;

public class SqlBonusProductRepository : BaseSqlRepository, IBonusProductRepository
{
    private readonly StoreAppDbContext _context;

    public SqlBonusProductRepository(StoreAppDbContext context, string connectionString)
        : base(connectionString)
    {
        _context = context;
    }

    public async Task AddAsync(BonusProduct entity)
    {
        await _context.BonusProducts.AddAsync(entity);
    }

    public  async Task DeleteAsync(int id)
    {
        var product = await _context.BonusProducts.FindAsync(id);
        if (product != null)
        {
            product.IsDeleted = true;
            product.DeletedDate = DateTime.UtcNow;
        }
    }

    public void Update(BonusProduct entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        _context.BonusProducts.Update(entity);
    }

    public IQueryable<BonusProduct> GetAll()
    {
        return _context.BonusProducts.Where(x => !x.IsDeleted);
    }

    public async Task<BonusProduct> GetByIdAsync(int id)
    {
        return await _context.BonusProducts
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }
}
