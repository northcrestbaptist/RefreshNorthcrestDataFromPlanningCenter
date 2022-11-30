using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Data;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
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

            _log.LogInformation("Retrieved the latest sermon record date/time from the Northcrest database, which is: {sermonDate}.", mostRecentSermonInDatabase);
            _log.LogInformation("Note the following sermon date rules:");
            _log.LogInformation("{rule1}", "(1) Sermon data refresh will look for services later than this date.");
            _log.LogInformation("{rule2}", "(2) For plans with future dates: Sermon data refresh will only pull the next 7 days into the Northcrest database.");
            return mostRecentSermonInDatabase;
        }
        
        public void AddSermonsToDatabase(IList<Sermon> sermons)
        {
            _log.LogInformation("Adding {numberOfSermons} plan(s) with 'Sermon' in the title to the database as sermon records.", sermons.Count);
            using var context = new NorthcrestDbContext();
            //context.AddRange(sermons);
            try
            {
                foreach (Sermon sermon in sermons)
                {
                    context.Add(sermon);
                }
            
                context.SaveChanges();
                _log.LogInformation("Added {numberOfSermons} sermon record(s) successfully.", sermons.Count);
            }
            catch(Exception ex)
            {
                _log.LogInformation("There was an error: {error}", ex);
            }
        }
        public void DeleteSermonsWithinDateRangeFromDatabase(int rangeDaysForDeletion)
        {
            TimeSpan daysToSubtract = new TimeSpan(rangeDaysForDeletion, 0, 0, 0, 0);
            DateTime deletionDateStart = DateTime.Now.Subtract(daysToSubtract);
            _log.LogInformation("Deleting sermon record(s) from the database with a Sermon Date/Time later than or equal to: {date}.  " +
                "These deleted record(s) will be repulled from Planning Center during this data refresh. ", deletionDateStart);
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
                _log.LogInformation("Deleted {number} sermon records(s) successfully.", numberDeleted);
            }
            catch(Exception ex)
            {
                _log.LogInformation("There was an error: {error}", ex);
            }
        }

        public void AddOrUpdateGeneralSong(GeneralSong generalSong)
        {
            using var context = new NorthcrestDbContext();
            try
            {
                GeneralSong generalSongToUpdate;
                if (generalSong.SongId > 0 && generalSong.ArrangementId > 0)
                {
                    generalSongToUpdate = context.GeneralSongs
                        .Where(song => song.SongId == generalSong.SongId
                            && song.ArrangementId == generalSong.ArrangementId)       
                        .FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(generalSong.SongName) && !string.IsNullOrEmpty(generalSong.ArrangementName))
                {
                    generalSongToUpdate = context.GeneralSongs
                        .Where(song => song.SongName == generalSong.SongName
                            && song.ArrangementName == generalSong.ArrangementName)
                        .FirstOrDefault();
                }
                else
                {
                    generalSongToUpdate = null;
                }

                if(generalSongToUpdate != null)
                {
                    generalSongToUpdate.LastScheduledDateTime = generalSong.LastScheduledDateTime;
                    generalSongToUpdate.Length = generalSong.Length;
                    generalSongToUpdate.Themes = generalSong.Themes;
                    context.Update(generalSongToUpdate);
                }
                else
                {
                    context.GeneralSongs.Add(generalSong);
                }
                
                context.SaveChanges();
                
            }
            catch (Exception ex)
            {
                _log.LogInformation("There was an error: {error}", ex);
            }
        }
    }
}

