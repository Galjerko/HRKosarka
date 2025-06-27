using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HRKošarka.Persistence.DatabaseContext
{
    public class HRDatabaseContextFactory
        : IDesignTimeDbContextFactory<HRDatabaseContext>
    {
        public HRDatabaseContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "HRKošarka.API"
            );

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<HRDatabaseContext>();
            var connectionString = configuration.GetConnectionString("HRDatabaseConnectionString");

            builder.UseSqlServer(connectionString);

            return new HRDatabaseContext(builder.Options);
        }
    }
}
