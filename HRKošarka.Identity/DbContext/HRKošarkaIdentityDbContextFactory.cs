using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HRKošarka.Identity.DbContext
{
    public class HRKošarkaIdentityDbContextFactory
            : IDesignTimeDbContextFactory<HRIdentityDbContext>
    {

        public HRIdentityDbContext CreateDbContext(string[] args)
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


            var builder = new DbContextOptionsBuilder<HRIdentityDbContext>();
            var connectionString = configuration.GetConnectionString("HRDatabaseConnectionString");

            builder.UseSqlServer(connectionString);

            return new HRIdentityDbContext(builder.Options);
        }
    }
}
