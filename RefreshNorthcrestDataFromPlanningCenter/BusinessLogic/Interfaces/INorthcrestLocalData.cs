using Northcrest.Domain.PlanningCenter;
using System;
using System.Collections.Generic;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces
{
    public  interface INorthcrestLocalData
    {
        DateTime GetLatestSermonDateTime(int futureDays);
        DateTime GetLatestPlanWithSongsDateTime(int futureDays);
        void AddSermonsToDatabase(IList<Sermon> sermons);
        void AddPlansForSongsToDatabase(IList<Plan_ForSongs> plan_ForSongsList);
        void DeleteSermonsWithinDateRangeFromDatabase(int numberOfDays);
        void DeleteAllPlan_ForSongRecordsFromDatabase();
        void AddOrUpdateGeneralSong(GeneralSong generalSong);
    }
}
