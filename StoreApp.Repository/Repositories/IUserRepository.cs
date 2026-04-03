using StoreApp.Domain.Entities;

namespace StoreApp.Repository.Repositories;

public interface IUserRepository
{
        Task AddAsync(User user);
        void Update(User user);
        Task DeleteAsync(int id);
        Task<User> GetByIdAsync(int id);
        IQueryable<User> GetAll();
}
