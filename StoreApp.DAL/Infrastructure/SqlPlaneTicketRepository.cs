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
    public void UpdateAsync(PlaneTicket planeticket)
    {
        var existingPlaneTicket = _context.PlaneTickets.Find(planeticket.Id);
        if (existingPlaneTicket != null)
        {
            existingPlaneTicket.Airline = planeticket.Airline;
            existingPlaneTicket.Gate = planeticket.Gate;
            existingPlaneTicket.Plane = planeticket.Plane;
            existingPlaneTicket.Meal = planeticket.Meal;
            existingPlaneTicket.HasCheckedIn = planeticket.HasCheckedIn;
            existingPlaneTicket.LuggageKg = planeticket.LuggageKg;
            existingPlaneTicket.VariantId = planeticket.VariantId;
            existingPlaneTicket.UpdatedDate = DateTime.UtcNow;
        }
    }
    public void DeleteAsync(int id)
    {
        var planeticket = _context.PlaneTickets.Find(id);
        if (planeticket != null)
        {
            _context.PlaneTickets.Remove(planeticket);
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