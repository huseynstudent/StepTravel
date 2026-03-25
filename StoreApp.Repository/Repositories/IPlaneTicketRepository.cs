using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface IPlaneTicketRepository
{
    Task AddAsync(PlaneTicket planeTicket);
    void Update(PlaneTicket planeTicket);
    Task DeleteAsync(int id);
    Task<PlaneTicket> GetByIdAsync(int id);
    IQueryable<PlaneTicket> GetAll();
}