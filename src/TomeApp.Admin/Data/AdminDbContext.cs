using Microsoft.EntityFrameworkCore;
using TomeApp.Admin.Models;

namespace TomeApp.Admin.Data;

public class AdminDbContext(DbContextOptions<AdminDbContext> options) : DbContext(options)
{
    public DbSet<UserView> Users => Set<UserView>();
    public DbSet<ChampionshipView> Championships => Set<ChampionshipView>();
    public DbSet<ReadingSessionView> ReadingSessions => Set<ReadingSessionView>();
}
