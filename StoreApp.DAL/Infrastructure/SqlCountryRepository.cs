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
        public void DeleteAsync(int id)
        {
            var country = _context.Countries.Find(id);

            if (country != null)
            {

                country.IsDeleted = true;
                country.DeletedDate = DateTime.UtcNow;
            }
        }
        public void UpdateAsync(Country entity)
        {
            var country = _context.Countries.Find(entity.Id);

            if (country != null)
            {
                country.Name = entity.Name;
                country.UpdatedDate = DateTime.UtcNow;
            }
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