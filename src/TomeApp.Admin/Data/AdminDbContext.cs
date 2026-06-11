using Microsoft.EntityFrameworkCore;
using TomeApp.Admin.Models;

namespace TomeApp.Admin.Data;

public class AdminDbContext(DbContextOptions<AdminDbContext> options) : DbContext(options)
{
    public DbSet<UserView> Users { get; set; } = null!;
    public DbSet<ChampionshipView> Championships { get; set; } = null!;
    public DbSet<ReadingSessionView> ReadingSessions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserView>(e =>
        {
            e.ToTable("Users");
            e.HasKey(u => u.Id);
        });

        modelBuilder.Entity<ChampionshipView>(e =>
        {
            e.ToTable("ChampionshipEntries");
            e.HasKey(c => c.Id);
            e.Ignore(c => c.UserName);
            e.Ignore(c => c.UserEmail);
        });

        modelBuilder.Entity<ReadingSessionView>(e =>
        {
            e.ToTable("ReadingSessions");
            e.HasKey(r => r.Id);
        });
    }
}
