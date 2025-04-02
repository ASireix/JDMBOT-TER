using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BotJDM.Database
{
    public class BotDBContextFactory : IDesignTimeDbContextFactory<BotDBContext>
    {
        public BotDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BotDBContext>();
            optionsBuilder.UseSqlServer("Server=tcp:botterserver.database.windows.net,1433;Initial Catalog=BotTerDb;Persist Security Info=False;User ID=sqladmin;Password=K6Lsg4GGRsC5;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            return new BotDBContext(optionsBuilder.Options);
        }
    }
}
