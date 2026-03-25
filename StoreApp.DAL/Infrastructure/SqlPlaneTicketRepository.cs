using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;

namespace StoreApp.DAL.Infrastructure;

public class SqlPlaneTicketRepository : BaseSqlRepository, IPlaneTicketRepository

{
    private readonly StoreAppDbContext _context;
    public SqlPlaneTicketRepository(StoreAppDbContext context, string connectionString) : base(connectionString)
    {
        _context = context;
    }
    public async Task AddAsync(PlaneTicket planeticket)
    {
        _context.PlaneTickets.Add(planeticket);
    }
    public void Update(PlaneTicket planeticket)
    {
        planeticket.UpdatedDate = DateTime.UtcNow;
            _context.PlaneTickets.Update(planeticket);
    }
    public async Task DeleteAsync(int id)
    {
        var planeticket = await _context.PlaneTickets.FindAsync(id);
        if (planeticket != null)
        {
            planeticket.IsDeleted = true;
            planeticket.DeletedDate = DateTime.UtcNow;
        }
    }
    public IQueryable<PlaneTicket> GetAll()
    {
        return _context.PlaneTickets.Where(s => !s.IsDeleted);
    }
    public async Task<PlaneTicket> GetByIdAsync(int id)
    {
        return await _context.PlaneTickets.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }
}