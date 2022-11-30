using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces
{
    public interface IRetrievePlanSongDataService
    {
        void GetPlanSongDataForSpecifiedPlan(AvailablePlansInformation availablePlansInformation, Plan plan);
    }
}
