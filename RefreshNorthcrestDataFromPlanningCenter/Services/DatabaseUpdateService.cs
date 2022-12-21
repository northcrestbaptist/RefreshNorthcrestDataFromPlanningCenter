using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using System.IO;
using System;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{
    public class DatabaseUpdateService: IDatabaseUpdateService
    {
        private readonly INorthcrestLocalData _northcrestLocalData;

        public DatabaseUpdateService(INorthcrestLocalData northcrestLocalData)
        {
            _northcrestLocalData = northcrestLocalData;
        }

        public void DeleteLocalDataToBeRefreshed(AvailablePlansInformation availablePlansInformation)
        {
            if (availablePlansInformation.RefreshAppConfig.RefreshPlanSongData)
            {
                _northcrestLocalData.DeleteAllPlan_ForSongRecordsFromDatabase();
                // Delete all files in a directory    
                string[] files = Directory.GetFiles(ServiceConstants.FILE_PATH_SONG_AUDIO);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }

            if (availablePlansInformation.RefreshAppConfig.RefreshSermonData)
            {
                _northcrestLocalData.DeleteSermonsWithinDateRangeFromDatabase(availablePlansInformation.RefreshAppConfig.NumberOfDaysToRefreshSermonData);
                availablePlansInformation.MostRecentSermonInNorthcrestDatabase = _northcrestLocalData.GetLatestSermonDateTime(availablePlansInformation.RefreshAppConfig.NumberOfDaysToRefreshFutureData);
            }
        }

        public void RefreshDataThatWasRetrievedFromPlanningCenter(AvailablePlansInformation availablePlansInformation)
        {
            if (availablePlansInformation.RefreshAppConfig.RefreshPlanSongData)
            {
                 _northcrestLocalData.AddPlansForSongsToDatabase(availablePlansInformation.Plan_ForSongsList);
            }

            if (availablePlansInformation.RefreshAppConfig.RefreshSermonData)
            {
                _northcrestLocalData.AddSermonsToDatabase(availablePlansInformation.Sermons);
            }
        }

        public void AddOrUpdateGeneralSong(GeneralSong generalSong)
        {
            _northcrestLocalData.AddOrUpdateGeneralSong(generalSong);
        }
    }
}
