using Microsoft.EntityFrameworkCore;
using StoreApp.DAL.Context;
using StoreApp.Domain.Entities;
using StoreApp.Repository.Repositories;
namespace StoreApp.DAL.Infrastructure
{
    public class SqlCountryRepository : BaseSqlRepository ,ICountryRepository
    {
        private readonly StoreAppDbContext _context;

        public SqlCountryRepository(StoreAppDbContext context, string connectionString) : base(connectionString)
        {
            _context = context;
        }
        public async Task AddAsync(Country entity)
        {
            _context.Countries.Add(entity);
        }
        public async Task DeleteAsync(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country != null)
            {

                country.IsDeleted = true;
                country.DeletedDate = DateTime.UtcNow;
            }
        }
        public void Update(Country entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
                _context.Countries.Update(entity);
        }
        public IQueryable<Country> GetAll()
        {
            return _context.Countries.Where(c => !c.IsDeleted);
        }
        public async Task<Country> GetByIdAsync(int id)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}