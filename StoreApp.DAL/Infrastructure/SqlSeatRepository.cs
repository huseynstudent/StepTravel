using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;

namespace StoreApp.DAL.Infrastructure;

public class SqlSeatRepository: BaseSqlRepository, ISeatRepository
{
    private readonly StoreAppDbContext _context;
    public SqlSeatRepository(StoreAppDbContext context, string connectionString) : base(connectionString)
    {
        _context = context;
    }
     public async Task AddAsync(Seat seat)
    {
        _context.Seats.Add(seat);
    }
    public void Update(Seat seat)
    {
        seat.UpdatedDate = DateTime.UtcNow;
        _context.Seats.Update(seat);
    }
    public async Task DeleteAsync(int id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat != null)
        {
            seat.IsDeleted = true;
            seat.DeletedDate = DateTime.UtcNow;
        }
    }
    public IQueryable<Seat> GetAll()
    {
        return _context.Seats.Where(s => !s.IsDeleted);
    }
    public async Task<Seat> GetByIdAsync(int id)
    {
        return await _context.Seats.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }
}
