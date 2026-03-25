using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;

namespace StoreApp.DAL.Infrastructure;

public class SqlTrainTicketRepository:BaseSqlRepository, ITrainTicketRepository

{
    private readonly StoreAppDbContext _context;
    public SqlTrainTicketRepository(StoreAppDbContext context, string connectionString) : base(connectionString)
    {
        _context = context;
    }
    public async Task AddAsync(TrainTicket trainticket)
    {
        _context.TrainTickets.Add(trainticket);
    }
    public void Update(TrainTicket trainticket)
    {
        trainticket.UpdatedDate = DateTime.UtcNow;
            _context.TrainTickets.Update(trainticket);
    }
    public async Task DeleteAsync(int id)
    {
        var trainticket = await _context.TrainTickets.FindAsync(id);
        if (trainticket != null)
        {
            trainticket.IsDeleted = true;
            trainticket.DeletedDate = DateTime.UtcNow;
        }
    }
    public IQueryable<TrainTicket> GetAll()
    {
        return _context.TrainTickets.Where(s => !s.IsDeleted);
    }
    public async Task<TrainTicket> GetByIdAsync(int id)
    {
        return await _context.TrainTickets.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }
}