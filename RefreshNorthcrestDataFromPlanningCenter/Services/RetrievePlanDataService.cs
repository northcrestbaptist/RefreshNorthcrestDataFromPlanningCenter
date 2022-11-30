using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using Newtonsoft.Json;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{
    public class RetrievePlanDataService : IRetrievePlanDataService
    {
        private readonly IRetrieveSermonDataService _retrieveSermonDataService;
        private readonly IRetrieveGeneralSongDataService _retrieveGeneralSongDataService;
        private readonly IRetrievePlanSongDataService _retrievePlanSongDataService;

        public RetrievePlanDataService(IRetrieveSermonDataService retrieveSermonDataService,
            IRetrieveGeneralSongDataService retrieveGeneralSongDataService,
            IRetrievePlanSongDataService retrievePlanSongDataService)
        {
            _retrieveSermonDataService = retrieveSermonDataService;
            _retrieveGeneralSongDataService = retrieveGeneralSongDataService;
            _retrievePlanSongDataService = retrievePlanSongDataService;
        }

        public void GetAvailablePlansForServiceType(AvailablePlansInformation plansInformation)
        {
            plansInformation.OffSet = 0;
            var responseTask = plansInformation.Client.GetAsync(plansInformation.CurrentUrl);
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

        public void GetDetailsForAllPlansForServiceType(AvailablePlansInformation plansInformation)
        {
            while (plansInformation.RemainingRecords > 0)
            {
                GetNextPageOfPlans(plansInformation);
                foreach (Plan plan in plansInformation.CurrentRetrievedPlans.data)
                {
                    if(plansInformation.RefreshAppConfig.RefreshSermonData)
                    {
                        _retrieveSermonDataService.GetSermonDataForSpecifiedPlan(plansInformation, plan);
                    }

                    if (plansInformation.RefreshAppConfig.RefreshPlanSongData)
                    {
                        //_retrievePlanSongDataService.GetPlanSongDataForSpecifiedPlan(plansInformation, plan);
                    }

                    if (plansInformation.RefreshAppConfig.RefreshGeneralSongData)
                    {
                        _retrieveGeneralSongDataService.GetGeneralSongDataForSpecifiedPlan(plansInformation, plan);
                    }

                }
                plansInformation.RemainingRecords -= plansInformation.PlanningCtrConfig.RateLimit;
                plansInformation.OffSet += plansInformation.PlanningCtrConfig.RateLimit;
            }
        }

        private void GetNextPageOfPlans(AvailablePlansInformation plansInformation)
        {
            string getPlansByPage =
                    $"{plansInformation.CurrentUrl}?offset={plansInformation.OffSet}&per_page={plansInformation.PlanningCtrConfig.RateLimit}";

            plansInformation.NumberOfRequests++;
            var responseTask = plansInformation.Client.GetAsync(getPlansByPage);
            plansInformation.NumberOfRequests++;
            responseTask.Wait();
            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            readTask.Wait();
            string planningCenterResults = readTask.Result;
            plansInformation.CurrentRetrievedPlans = JsonConvert.DeserializeObject<Plans>(planningCenterResults);
        }
    }
}
