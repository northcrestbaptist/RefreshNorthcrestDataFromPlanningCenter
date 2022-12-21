using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces
{
    public interface IRetrievePlanningCenterDataService
    {
        Task RunAsync();
    }
}