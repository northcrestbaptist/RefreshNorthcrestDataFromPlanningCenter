using Microsoft.EntityFrameworkCore;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Data
{
    public class NorthcrestDbContext : DbContext
    {
        public DbSet<Sermon> Sermons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(
                "Data Source=(localhost)\\NBCPLANNINGCTR;Initial Catalog=NBCDB"

            );
        }
    }
}
