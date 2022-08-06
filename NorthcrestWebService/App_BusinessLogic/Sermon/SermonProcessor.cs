using Microsoft.EntityFrameworkCore;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Data;

namespace NorthcrestWebService.App_BusinessLogic.ManifestUnit
{
    public class SermonProcessor : ISermonProcessor
    {

        public SermonProcessor() { }

        public async Task<IList<Sermon>> GetSermonsAsync()
        {
            using var context = new NorthcrestDbContext();
            IList<Sermon> sermons = await context.Sermons.OrderByDescending(sermon => sermon.SermonDateTime).ToListAsync();

            return sermons;
        }
    }
}