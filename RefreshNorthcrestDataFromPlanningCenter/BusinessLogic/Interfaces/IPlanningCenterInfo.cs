using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using System.Net.Http;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces
{
    public interface IPlanningCenterInfo
    {
        PlanningCenterConfiguration GetPlanningCenterConfiguration(HttpClient client);
    }
}
