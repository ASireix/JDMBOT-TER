using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BotJDM.Database
{
    public class BotDBContextFactory : IDesignTimeDbContextFactory<BotDBContext>
    {
        public BotDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BotDBContext>();
            var projectRoot = AppContext.BaseDirectory.Contains("bin")
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."))
            : AppContext.BaseDirectory;

            var dbPath = Path.Combine(projectRoot, "botdata.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new BotDBContext(optionsBuilder.Options);
        }
    }
}
