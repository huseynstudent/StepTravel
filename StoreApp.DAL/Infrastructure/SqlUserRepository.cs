using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Repository.Repositories;

namespace StoreApp.DAL.Infrastructure;

public class SqlUserRepository:BaseSqlRepository, IUserRepository
{
    private readonly StoreAppDbContext _context;
    public SqlUserRepository(StoreAppDbContext context, string connectionString) : base(connectionString)
    {
        _context = context;
    }
    public async Task AddAsync(Domain.Entities.User entity)
    {
        _context.Users.Add(entity);
    }
    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsDeleted = true;
            user.DeletedDate = DateTime.UtcNow;
        }
    }
    public void Update(Domain.Entities.User entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        _context.Users.Update(entity);
    }
    public IQueryable<Domain.Entities.User> GetAll()
    {
        return _context.Users.Where(u => !u.IsDeleted);
    }
    public async Task<Domain.Entities.User> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }
}
