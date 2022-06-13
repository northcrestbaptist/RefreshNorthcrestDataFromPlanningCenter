using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces
{
    public interface IGetSermonPlanDetails
    {
        void GetDetailsForAllPlansForServiceType(AvailablePlansInformation availablePlansInfo);

    }
}
