using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;

namespace StoreApp.DAL.Infrastructure;

public class SqlMessageRepository : BaseSqlRepository, IMessageRepository
{
    private readonly StoreAppDbContext _context;

    public SqlMessageRepository(StoreAppDbContext context, string connectionString) : base(connectionString)
    {
        _context = context;
    }

    public async Task AddAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
    }

    public void Update(Message message)
    {
        message.UpdatedDate = DateTime.UtcNow;
        _context.Messages.Update(message);
    }

    public async Task DeleteAsync(int id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message != null)
        {
            message.IsDeleted = true;
            message.DeletedDate = DateTime.UtcNow;
        }
    }

    public IQueryable<Message> GetAll()
    {
        return _context.Messages.Where(m => !m.IsDeleted).Include(m => m.Sender);
    }

    public async Task<Message> GetByIdAsync(int id)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
    }
}
