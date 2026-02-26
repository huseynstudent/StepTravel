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
    public void DeleteAsync(int id)
    {
        var variant = _context.Variants.Find(id);

        if (variant != null)
        {
            _context.Variants.Remove(variant);
        }
    }
    public void UpdateAsync(Variant entity)
    {
        var variant = _context.Variants.Find(entity.Id);

        if (variant != null)
        {
            variant.Name = entity.Name;
            variant.Price = entity.Price;
            variant.AllowedLuggageKg = entity.AllowedLuggageKg;
            variant.AllowedLuggageCount = entity.AllowedLuggageCount;
            variant.IsPriority = entity.IsPriority;
            _context.Variants.Update(variant);
        }
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