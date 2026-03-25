using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface ITrainTicketRepository
{
    Task AddAsync(TrainTicket trainTicket);
    void Update(TrainTicket trainTicket);
    Task DeleteAsync(int id);
    Task<TrainTicket> GetByIdAsync(int id);
    IQueryable<TrainTicket> GetAll();
}