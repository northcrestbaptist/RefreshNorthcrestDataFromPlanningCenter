using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces
{
    public interface IGetSermonPlans
    {
        void GetAvailablePlansForServiceType(AvailablePlansInformation availablePlansInformation);
    }
}
