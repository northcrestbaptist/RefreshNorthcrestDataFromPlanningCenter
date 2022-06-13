using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Data;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic
{
    public class NorthcrestLocalData : INorthcrestLocalData
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        public NorthcrestLocalData (ILogger<RetrievePlanningCenterDataService> log)
        {
            _log = log;
        }

        public DateTime GetLatestSermonDateTime()
        {
            DateTime mostRecentSermonInDatabase;
            using var context = new NorthcrestDbContext();
            int recordCount = context.Sermons.ToList().Count();
            if (recordCount > 0)
            {
                mostRecentSermonInDatabase = context.Sermons.Max(o => o.SermonDateTime);
            }
            else
            {
                mostRecentSermonInDatabase = DateTime.MinValue;
            }

            _log.LogInformation("Retrieved the latest plan date/time, which is: {planDate}", mostRecentSermonInDatabase);
            return mostRecentSermonInDatabase;
        }
        
        public void AddSermonsToDatabase(IList<Sermon> sermons)
        {
            _log.LogInformation("Adding {numberOfSermons} plans with 'Sermon' in the title to the database.", sermons.Count);
            using var context = new NorthcrestDbContext();
            context.AddRange(sermons);
            context.SaveChanges();
        }
    }
}

