using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;

        public PlanningCenterInfo(
            
            ILogger<RetrievePlanningCenterDataService> log,
            IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public PlanningCenterConfiguration GetPlanningCenterConfiguration(HttpClient client)
        {
            string url = _config.GetValue<string>(ServiceConstants.SUNDAY_MORNING_PLANS_URL_CONFIG);
            string rateLimit = _config.GetValue<string>(ServiceConstants.RATE_LIMIT_REQUEST);
            string ratePeriod = _config.GetValue<string>(ServiceConstants.RATE_PERIOD_REQUEST);
            PlanningCenterConfiguration configuration = new();
            var responseTask = client.GetAsync(url);
            responseTask.Wait();
            var result = responseTask.Result;
            int parseResult;
            bool parseSuccess;
            parseSuccess = int.TryParse(result.Headers.GetValues(rateLimit).FirstOrDefault(), out parseResult);
            configuration.RateLimit = parseSuccess ? parseResult : 100;
            _log.LogInformation("Retrieved rate limit. {rateLimit}", configuration.RateLimit);
            parseSuccess = int.TryParse(result.Headers.GetValues(ratePeriod).FirstOrDefault(), out parseResult);
            configuration.RatePeriod = parseSuccess ? parseResult : 20;
            _log.LogInformation("Retrieved rate period. {ratePeriod}", configuration.RatePeriod);
            return configuration;
        }
    }
}
