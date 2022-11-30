using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;

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
                // Add DeletePlanSongsWithinDateRangeFromDatabase(availablePlansInformation.RefreshAppConfig.NumberOfDaysToRefreshPlanSongData);
                // call... once developed.
                // Add this also: availablePlansInformation.MostRecentPlanSongInfoInNorthcrestDatabase = _northcrestLocalData.GetLatestPlanSongDateTime();
            }

            if (availablePlansInformation.RefreshAppConfig.RefreshSermonData)
            {
                _northcrestLocalData.DeleteSermonsWithinDateRangeFromDatabase(availablePlansInformation.RefreshAppConfig.NumberOfDaysToRefreshSermonData);
                availablePlansInformation.MostRecentSermonInNorthcrestDatabase = _northcrestLocalData.GetLatestSermonDateTime();
            }
        }

        public void RefreshDataThatWasRetrievedFromPlanningCenter(AvailablePlansInformation availablePlansInformation)
        {
            if (availablePlansInformation.RefreshAppConfig.RefreshPlanSongData)
            {
                // Add _northcrestLocalData.AddPlanSongsToDatabase(availablePlansInformation.PlanSongs);
                // call... once developed.
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
