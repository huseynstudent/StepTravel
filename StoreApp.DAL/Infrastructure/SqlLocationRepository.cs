using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;
namespace StoreApp.DAL.Infrastructure;
public class SqlLocationRepository : BaseSqlRepository, ILocationRepository 
{
    private readonly StoreAppDbContext _context;
    public SqlLocationRepository(StoreAppDbContext context, string connectionString) : base(connectionString)
    {
        _context = context;
    }
    public async Task AddAsync(Location entity)
    {
        _context.Locations.Add(entity);
    }
    public void DeleteAsync(int id)
    {
        var location = _context.Locations.Find(id);

        if (location != null)
        {
            _context.Locations.Remove(location);
            location.IsDeleted = true;
            location.DeletedDate = DateTime.UtcNow;
        }
    }
    public void UpdateAsync(Location entity)
    {
        var location = _context.Locations.Find(entity.Id);

        if (location != null)
        {
            location.Name = entity.Name;
            location.CountryId = entity.CountryId;
            location.UpdatedDate = DateTime.UtcNow;
        }
    }
    public IQueryable<Location> GetAll()
    {
        return _context.Locations.Where(l => !l.IsDeleted);
    }
    public async Task<Location> GetByIdAsync(int id)
    {
        return await _context.Locations.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
    }
}