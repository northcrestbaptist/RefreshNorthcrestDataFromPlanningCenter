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

            _log.LogInformation("Retrieved the latest plan date/time from the Northcrest database, which is: {planDate}.  Note the following date rules: (1) Data refresh will look for services later than this date. (2) Data refresh will not pull future services into the Northcrest database. ", mostRecentSermonInDatabase);
            return mostRecentSermonInDatabase;
        }
        
        public void AddSermonsToDatabase(IList<Sermon> sermons)
        {
            _log.LogInformation("Adding {numberOfSermons} plan(s) with 'Sermon' in the title to the database.", sermons.Count);
            using var context = new NorthcrestDbContext();
            //context.AddRange(sermons);
            try
            {
                foreach (Sermon sermon in sermons)
                {
                    context.Add(sermon);
                }
            
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                _log.LogInformation("There was an error: {error}", ex);
            }
        }
    }
}

