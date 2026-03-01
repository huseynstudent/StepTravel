using StoreApp.Domain.Entities;

namespace StoreApp.Repository.Repositories;

public interface ISeatRepository
{
    Task AddAsync(Seat seat);
    void UpdateAsync(Seat seat);
    void DeleteAsync(int id);
    Task<Seat> GetByIdAsync(int id);
    IQueryable<Seat> GetAll();
}
