using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;

namespace StoreApp.DAL.Infrastructure;

public class SqlLocationRepository : BaseSqlRepository, ILocationRepository
{
    private readonly StoreAppDbContext _context;

    public SqlLocationRepository(StoreAppDbContext context, string connectionString)
        : base(connectionString)
    {
        _context = context;
    }

    public async Task AddAsync(Location entity)
    {
        await _context.Locations.AddAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location != null)
        {
            location.IsDeleted = true;
            location.DeletedDate = DateTime.UtcNow;
        }
    }

    public void Update(Location entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        _context.Locations.Update(entity);
    }
    public IQueryable<Location> GetAll()
    {
        return _context.Locations
            .Include(l => l.Country)
            .Where(l => !l.IsDeleted);
    }
    public async Task<Location> GetByIdAsync(int id)
    {
        return await _context.Locations
            .Include(l => l.Country)
            .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
    }
}