using StoreApp.Domain.Entities;

namespace StoreApp.Repository.Repositories;

public interface IMessageRepository
{
    Task AddAsync(Message message);
    void Update(Message message);
    Task DeleteAsync(int id);
    Task<Message> GetByIdAsync(int id);
    IQueryable<Message> GetAll();
}
