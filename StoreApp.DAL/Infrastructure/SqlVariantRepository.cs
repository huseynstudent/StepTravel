using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;
namespace StoreApp.DAL.Infrastructure;
public class SqlVariantRepository : BaseSqlRepository, IVariantRepository
{
    private readonly StoreAppDbContext _context;
    public SqlVariantRepository(StoreAppDbContext context, string connectionString) : base(connectionString)
    {
        _context = context;
    }
    public async Task AddAsync(Variant entity)
    {
        _context.Variants.Add(entity);
    }
    public async Task DeleteAsync(int id)
    {
        var variant = await _context.Variants.FindAsync(id);

        if (variant != null)
        {
            variant.IsDeleted = true;
            variant.DeletedDate = DateTime.UtcNow;

        }
    }
    public void Update(Variant entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        _context.Variants.Update(entity);
    }
    public IQueryable<Variant> GetAll()
    {
        return _context.Variants.Where(v => !v.IsDeleted);
    }
    public async Task<Variant> GetByIdAsync(int id)
    {
        return await _context.Variants.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
    }
}