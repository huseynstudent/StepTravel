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
    public DbSet<BonusProduct> BonusProducts { get; set; }
    public DbSet<BonusCard> BonusCards { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<TrainTicket> TrainTickets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PLANE TICKET
        modelBuilder.Entity<PlaneTicket>()
            .HasOne(p => p.From)
            .WithMany()
            .HasForeignKey(p => p.FromId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PlaneTicket>()
            .HasOne(p => p.To)
            .WithMany()
            .HasForeignKey(p => p.ToId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PlaneTicket>()
            .HasOne(p => p.Variant)
            .WithMany()
            .HasForeignKey(p => p.VariantId)
            .OnDelete(DeleteBehavior.NoAction);

        // PlaneTicket → chosen seat (user's purchase decision)
        modelBuilder.Entity<PlaneTicket>()
            .HasOne<Seat>()
            .WithMany()
            .HasForeignKey(p => p.ChosenSeatId)
            .OnDelete(DeleteBehavior.NoAction);

        // TRAIN TICKET
        modelBuilder.Entity<TrainTicket>()
            .HasOne(t => t.From)
            .WithMany()
            .HasForeignKey(t => t.FromId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TrainTicket>()
            .HasOne(t => t.To)
            .WithMany()
            .HasForeignKey(t => t.ToId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TrainTicket>()
            .HasOne(t => t.Variant)
            .WithMany()
            .HasForeignKey(t => t.VariantId)
            .OnDelete(DeleteBehavior.NoAction);

        // TrainTicket → chosen seat (user's purchase decision)
        modelBuilder.Entity<TrainTicket>()
            .HasOne<Seat>()
            .WithMany()
            .HasForeignKey(t => t.ChosenSeatId)
            .OnDelete(DeleteBehavior.NoAction);

        // SEAT → ticket pool ownership (seat belongs to a flight/train)
        modelBuilder.Entity<Seat>()
            .HasOne<PlaneTicket>()
            .WithMany()
            .HasForeignKey(s => s.PlaneTicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Seat>()
            .HasOne<TrainTicket>()
            .WithMany()
            .HasForeignKey(s => s.TrainTicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}