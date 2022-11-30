using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;

namespace RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces
{
    public interface IRetrieveItemsService
    {
        void GetItemsForSpecifiedPlan(AvailablePlansInformation plansInformation, Plan plan);
    }
}
