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
    public void UpdateAsync(TrainTicket trainticket)
    {
        var existingTrainTicket = _context.TrainTickets.Find(trainticket.Id);
        if (existingTrainTicket != null)
        {
            existingTrainTicket.TrainCompany = trainticket.TrainCompany;
            existingTrainTicket.TrainNumber = trainticket.TrainNumber;
            existingTrainTicket.VagonNumber = trainticket.VagonNumber;
            existingTrainTicket.VariantId = trainticket.VariantId;
            existingTrainTicket.UpdatedDate = DateTime.UtcNow;
        }
    }
    public void DeleteAsync(int id)
    {
        var trainticket = _context.TrainTickets.Find(id);
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