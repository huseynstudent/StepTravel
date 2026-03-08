using Microsoft.EntityFrameworkCore;
using StoreApp.Domain.Entities;

namespace StoreApp.DAL.Context;

public class StoreAppDbContext:DbContext
{
    public StoreAppDbContext(DbContextOptions<StoreAppDbContext> options) : base(options)
    {

    }
    public DbSet<PlaneTicket> PlaneTickets { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Variant> Variants { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<BonusCard> BonusCards { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<TrainTicket> TrainTickets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
}