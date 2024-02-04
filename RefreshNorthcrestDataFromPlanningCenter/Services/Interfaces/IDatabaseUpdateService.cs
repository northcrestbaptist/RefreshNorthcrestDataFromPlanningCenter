using Northcrest.Domain.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces
{
    public interface IDatabaseUpdateService
    {
        void DeleteLocalDataToBeRefreshed(AvailablePlansInformation availablePlansInfo);
        void RefreshDataThatWasRetrievedFromPlanningCenter(AvailablePlansInformation availablePlansInfo);
        void AddOrUpdateGeneralSong(GeneralSong generalSong);
    }
}
