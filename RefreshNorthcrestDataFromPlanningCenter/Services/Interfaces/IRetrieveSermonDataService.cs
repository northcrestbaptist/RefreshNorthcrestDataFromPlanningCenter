using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces
{
    public interface IRetrieveSermonDataService
    {
        void GetSermonDataForSpecifiedPlan(AvailablePlansInformation availablePlansInformation, Plan plan);

    }
}
