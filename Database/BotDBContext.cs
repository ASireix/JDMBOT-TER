using BotJDM.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // TrustFactor entre -100 et 100
            modelBuilder.Entity<UserEntity>()
                .Property(u => u.TrustFactor)
                .HasDefaultValue(0);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=tcp:botterserver.database.windows.net,1433;Initial Catalog=BotTerDb;Persist Security Info=False;User ID=sqladmin;Password=K6Lsg4GGRsC5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }
    }
}
