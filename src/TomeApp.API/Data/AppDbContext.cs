using Microsoft.EntityFrameworkCore;
using TomeApp.API.Models.Entities;

namespace TomeApp.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<ReadingSession> ReadingSessions => Set<ReadingSession>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<ChampionshipEntry> ChampionshipEntries => Set<ChampionshipEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Region).HasDefaultValue("BR");
        });

        modelBuilder.Entity<Book>(e =>
        {
            e.HasKey(b => b.Id);
            e.HasOne(b => b.User).WithMany(u => u.Books).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade);
            e.Property(b => b.Status).HasConversion<string>();
        });

        modelBuilder.Entity<ReadingSession>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasOne(r => r.User).WithMany(u => u.ReadingSessions).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Book).WithMany(b => b.ReadingSessions).HasForeignKey(r => r.BookId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Note>(e =>
        {
            e.HasKey(n => n.Id);
            e.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(n => n.Book).WithMany(b => b.Notes).HasForeignKey(n => n.BookId).OnDelete(DeleteBehavior.Cascade);
            e.Property(n => n.Type).HasConversion<string>();
        });

        modelBuilder.Entity<ChampionshipEntry>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(c => new { c.UserId, c.Year, c.Month, c.Region }).IsUnique();
        });
    }
}
