using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic
{
    
    public class GetSermonPlans : IGetSermonPlans
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        public GetSermonPlans(ILogger<RetrievePlanningCenterDataService> log)
        {
            _log = log;
        }

        public void GetAvailablePlansForServiceType(AvailablePlansInformation plansInformation)
        {
            plansInformation.OffSet = 0;
            var responseTask = plansInformation.Client.GetAsync(plansInformation.Url);
            responseTask.Wait();

            var result = responseTask.Result;
            var readPlanForTotalCountTask = result.Content.ReadAsStringAsync();
            readPlanForTotalCountTask.Wait();
            string firstResults = readPlanForTotalCountTask.Result;
            plansInformation.CurrentRetrievedPlans = JsonConvert.DeserializeObject<Plans>(firstResults);
            plansInformation.TotalRecordsAvailable = plansInformation.CurrentRetrievedPlans.meta.total_count;
            plansInformation.RemainingRecords = plansInformation.CurrentRetrievedPlans.meta.total_count;
            //_log.LogInformation("Obtained information for {serviceType} plans.", plansInformation.Type);
            //_log.LogInformation("Total plans available. {totalPlansAvailable}", plansInformation.CurrentRetrievedPlans.meta.total_count);
        }
    }
}
