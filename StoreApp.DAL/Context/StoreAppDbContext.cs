using Microsoft.EntityFrameworkCore;
using StoreApp.Domain.Entities;

namespace StoreApp.DAL.Context;

public class StoreAppDbContext:DbContext
{
    public StoreAppDbContext(DbContextOptions<StoreAppDbContext> options) : base(options)
    {

    }
    DbSet<PlaneTicket> PlaneTickets { get; set; }
    DbSet<Seat> Seats { get; set; }
    DbSet<Variant> Variants { get; set; }
    DbSet<Location> Locations { get; set; }
    DbSet<BonusCard> BonusCards { get; set; }
    DbSet<Country> Countries { get; set; }
    DbSet<TrainTicket> TrainTickets { get; set; }

}
