using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;

namespace RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces
{
    public interface IRetrievePlanDataService
    {
        void GetAvailablePlansForServiceType(AvailablePlansInformation plansInformation);

        void GetDetailsForAllPlansForServiceType(AvailablePlansInformation plansInformation);
    }
}
