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
    public void UpdateAsync(Seat seat)
    {
        var existingSeat = _context.Seats.Find(seat.Id);
        if (existingSeat != null)
        {
            existingSeat.Name = seat.Name;
            existingSeat.IsOccupied = seat.IsOccupied;
            existingSeat.VariantId = seat.VariantId;
            existingSeat.UpdatedDate = DateTime.UtcNow;
        }
    }
    public void DeleteAsync(int id)
    {
        var seat = _context.Seats.Find(id);
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
