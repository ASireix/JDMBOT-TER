using BotJDM.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BotJDM.Database
{
    public class BotDBContext : DbContext
    {
        public DbSet<NodeEntity> Nodes { get; set; }
        public DbSet<RelationEntity> Relations { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        public BotDBContext(DbContextOptions<BotDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NodeEntity>().ToTable("Nodes");
            modelBuilder.Entity<RelationEntity>().ToTable("Relations");
            modelBuilder.Entity<UserEntity>().ToTable("Users");

            modelBuilder.Entity<UserEntity>()
                .Property(u => u.TrustFactor)
                .HasDefaultValue(0);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var projectRoot = AppContext.BaseDirectory;

                var dbPath = Path.Combine(projectRoot, "botdata.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }
    }
}
