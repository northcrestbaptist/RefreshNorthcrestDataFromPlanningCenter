using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services;
using System.Linq;
using System.Net.Http;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic
{
    public class PlanningCenterInfo : IPlanningCenterInfo
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;

        public PlanningCenterInfo(ILogger<RetrievePlanningCenterDataService> log)
        {
            _log = log;
        }

        public PlanningCenterConfiguration GetPlanningCenterConfiguration(HttpClient client)
        {
            PlanningCenterConfiguration configuration = new();
            var responseTask = client.GetAsync(ServiceConstants.SUNDAY_MORNING_SERVICE_PLANS_URL);
            responseTask.Wait();
            var result = responseTask.Result;
            int parseResult;
            bool parseSuccess;
            parseSuccess = int.TryParse(result.Headers.GetValues(ServiceConstants.RATE_LIMIT_REQUEST).FirstOrDefault(), out parseResult);
            configuration.RateLimit = parseSuccess ? parseResult : 100;
            _log.LogInformation("Retrieved rate limit. {rateLimit}", configuration.RateLimit);
            parseSuccess = int.TryParse(result.Headers.GetValues(ServiceConstants.RATE_PERIOD_REQUEST).FirstOrDefault(), out parseResult);
            configuration.RatePeriod = parseSuccess ? parseResult : 20;
            _log.LogInformation("Retrieved rate period. {ratePeriod}", configuration.RatePeriod);
            return configuration;
        }
    }
}
