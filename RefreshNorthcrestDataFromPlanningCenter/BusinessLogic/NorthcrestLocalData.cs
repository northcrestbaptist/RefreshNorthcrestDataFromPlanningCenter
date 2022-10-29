using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Data;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic
{
    public class NorthcrestLocalData : INorthcrestLocalData
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        private readonly IConfiguration _config;
        public NorthcrestLocalData (ILogger<RetrievePlanningCenterDataService> log, IConfiguration config)
        {
            _log = log;
            _config = config;
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

            _log.LogInformation("Retrieved the latest plan date/time from the Northcrest database, which is: {planDate}.  Note the following date rules: (1) Data refresh will look for services later than this date. " +
                "(2) For plans with future dates: Data refresh will only pull the next 7 days into the Northcrest database. ", mostRecentSermonInDatabase);
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
                _log.LogInformation("Added {numberOfSermons} plans(s) successfully.", sermons.Count);
            }
            catch(Exception ex)
            {
                _log.LogInformation("There was an error: {error}", ex);
            }
        }
        public void DeleteSermonsWithinDateRangeFromDatabase()
        {
            int rangeDaysForDeletion = _config.GetValue<int>(ServiceConstants.NUMBER_OF_DAYS_TO_REFRESH);
            TimeSpan daysToSubtract = new TimeSpan(rangeDaysForDeletion, 0, 0, 0, 0);
            DateTime deletionDateStart = DateTime.Now.Subtract(daysToSubtract);
            _log.LogInformation("The refresh configuration is set to refresh the past {days} day(s). " +
                "Note this configuration can be changed in C:\\RefreshNorthcrestDatabase\\appsettings.json. " +
                "Only change the value of NumberOfDaysToRefresh. ", rangeDaysForDeletion);
            _log.LogInformation("Deleting plan(s) from the database with a Sermon Date/Time later than or equal to: {date}.  " +
                "These deleted plan(s) will be repulled from Planning Center during this data refresh. ", deletionDateStart);
            using var context = new NorthcrestDbContext();
            try
            {
                int numberDeleted = 0;
                var sermonsToRemove = context.Sermons.Where(sermon => sermon.SermonDateTime >= deletionDateStart);
                foreach (var sermon in sermonsToRemove)
                {
                    context.Remove(sermon);
                    numberDeleted++;
                }
                context.SaveChanges();
                _log.LogInformation("Deleted {number} plan(s) successfully.", numberDeleted);
            }
            catch(Exception ex)
            {
                _log.LogInformation("There was an error: {error}", ex);
            }
        }
    }
}

