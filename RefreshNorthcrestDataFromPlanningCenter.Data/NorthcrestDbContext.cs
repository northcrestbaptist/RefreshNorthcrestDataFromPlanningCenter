using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace RefreshNorthcrestDataFromPlanningCenter.Data
{
    public class NorthcrestDbContext : DbContext
    {
        public DbSet<Sermon> Sermons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder
                .UseSqlServer(GetConnectionStringForNorthcrestDbContext(),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: System.TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                })
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        private string GetConnectionStringForNorthcrestDbContext()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build();

            return configuration.GetConnectionString("Northcrest_DB_ConnectionString");
        }
    }
}
