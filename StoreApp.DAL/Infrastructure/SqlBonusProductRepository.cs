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

    public void DeleteAsync(int id)
    {
        var product = _context.BonusProducts.Find(id);
        if (product != null)
        {
            product.IsDeleted = true;
            product.DeletedDate = DateTime.UtcNow;
        }
    }

    public void UpdateAsync(BonusProduct entity)
    {
        var product = _context.BonusProducts.Find(entity.Id);
        if (product != null)
        {
            product.Name = entity.Name;
            product.PricePoint = entity.PricePoint;
            product.InStock = entity.InStock;
            product.UpdatedDate = DateTime.UtcNow;

            // Only replace image if a new one was provided
            if (!string.IsNullOrEmpty(entity.ImageUrl))
            {
                product.ImageUrl = entity.ImageUrl;
                product.ImageFileName = entity.ImageFileName;
            }
        }
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
