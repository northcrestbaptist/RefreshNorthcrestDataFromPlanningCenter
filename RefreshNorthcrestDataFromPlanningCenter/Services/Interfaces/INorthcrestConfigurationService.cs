using RefreshNorthcrestDataFromPlanningCenter.Models.Local;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces
{
    public interface INorthcrestConfigurationService
    {
        void SetConfiguration(AvailablePlansInformation availablePlansInfo);
        bool IsAnythingConfiguredToRefresh(AvailablePlansInformation availablePlans);
        void LogLocalAppConfigurationInstructions();
    }
}
