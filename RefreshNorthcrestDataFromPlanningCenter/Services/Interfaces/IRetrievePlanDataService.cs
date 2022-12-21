using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces
{
    public interface IRetrievePlanDataService
    {
        void GetAvailablePlansForServiceType(AvailablePlansInformation plansInformation);

        Task GetDetailsForAllPlansForServiceTypeAsync(AvailablePlansInformation plansInformation);
    }
}
