using StoreApp.Domain.Entities;
namespace StoreApp.Repository.Repositories;
public interface IPlaneTicketRepository
{
    Task AddAsync(PlaneTicket planeTicket);
    void UpdateAsync(PlaneTicket planeTicket);
    void DeleteAsync(int id);
    Task<PlaneTicket> GetByIdAsync(int id);
    IQueryable<PlaneTicket> GetAll();
}