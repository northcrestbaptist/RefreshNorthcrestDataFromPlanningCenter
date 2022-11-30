using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Models.Local;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{
    public class NorthcrestConfigurationService : INorthcrestConfigurationService
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        private readonly IConfiguration _config;

        public NorthcrestConfigurationService(ILogger<RetrievePlanningCenterDataService> log,
            IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public void SetConfiguration(AvailablePlansInformation availablePlansInfo)
        {
            availablePlansInfo.RefreshAppConfig = LoadLocalAppConfigurations();
            availablePlansInfo.Client.DefaultRequestHeaders.Authorization = LoadClientConfiguration();
            availablePlansInfo.PlanUrlList = LoadUrls();
            availablePlansInfo.PlanTypeList = LoadPlanTypes();
            availablePlansInfo.PlanningCtrConfig = LoadPlanningCenterConfiguration(availablePlansInfo);
            availablePlansInfo.NumberOfRequests++;
        }

        public bool IsAnythingConfiguredToRefresh(AvailablePlansInformation availablePlansInformation)
        {
            return availablePlansInformation.RefreshAppConfig.RefreshGeneralSongData
                || availablePlansInformation.RefreshAppConfig.RefreshPlanSongData
                || availablePlansInformation.RefreshAppConfig.RefreshPlanSongData;
        }

        public void LogLocalAppConfigurationInstructions()
        {
            _log.LogInformation("{configInstructions}", "Configuration Instructions:");
            _log.LogInformation("The app refresh configuration can be changed in: {configLocation}", "C:\\RefreshNorthcrestDatabase\\appsettings.json.");
            _log.LogInformation("The following values can be set to {true} or {false} to enable each refresh: " +
                "{RefreshGeneralSongData}, {RefreshPlanSongData}, and {RefreshSermonData}", true, false,
                ServiceConstants.REFRESH_GENERAL_SONG_DATA, ServiceConstants.REFRESH_PLAN_SONG_DATA, ServiceConstants.REFRESH_SERMON_DATA);
            _log.LogInformation("The following values can be set to a number to set the number of days to refresh: " +
                "{NumberOfDaysToRefreshGeneralSongData}, {NumberOfDaysToRefreshPlanSongData}, and {NumberOfDaysToRefreshSermonData}",
                ServiceConstants.NUMBER_OF_DAYS_TO_REFRESH_GENERAL_SONG_DATA, ServiceConstants.NUMBER_OF_DAYS_TO_REFRESH_PLAN_SONG_DATA,
                ServiceConstants.NUMBER_OF_DAYS_TO_REFRESH_SERMON_DATA);
        }

        private AuthenticationHeaderValue LoadClientConfiguration()
        {
            string appID = _config.GetValue<string>(ServiceConstants.APP_ID);
            string secret = _config.GetValue<string>(ServiceConstants.SECRET);
            string basic = _config.GetValue<string>(ServiceConstants.BASIC);
            var authenticationString = $"{appID}:{secret}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
            return new AuthenticationHeaderValue(basic, base64EncodedAuthenticationString);
        }

        private string[] LoadUrls()
        {
            string[] planUrlList = new string[3];
            string sundayMorningPlansUrl = _config.GetValue<string>(ServiceConstants.SUNDAY_MORNING_PLANS_URL_CONFIG);
            string sundayEveningPlansUrl = _config.GetValue<string>(ServiceConstants.SUNDAY_EVENING_PLANS_URL_CONFIG);
            string specialPlansUrl = _config.GetValue<string>(ServiceConstants.SPECIAL_PLANS_URL_CONFIG);
            planUrlList[0] = sundayMorningPlansUrl;
            planUrlList[1] = sundayEveningPlansUrl;
            planUrlList[2] = specialPlansUrl;
            return planUrlList;
        }

        private string[] LoadPlanTypes()
        {
            string[] planTypeList = new string[3];
            planTypeList[0] = ServiceConstants.SUNDAY_MORNING_SERVICE;
            planTypeList[1] = ServiceConstants.SUNDAY_EVENING_SERVICE;
            planTypeList[2] = ServiceConstants.SPECIAL_SERVICE;
            return planTypeList;
        }

        private LocalAppConfiguration LoadLocalAppConfigurations()
        {
            LocalAppConfiguration localAppConfig = new LocalAppConfiguration();
            _log.LogInformation("Retieving local refresh configurations...");
            localAppConfig.RefreshGeneralSongData = _config.GetValue<bool>(ServiceConstants.REFRESH_GENERAL_SONG_DATA);
            _log.LogInformation("Refresh General Song Data: {refresh}", localAppConfig.RefreshGeneralSongData);
            localAppConfig.RefreshPlanSongData = _config.GetValue<bool>(ServiceConstants.REFRESH_PLAN_SONG_DATA);
            _log.LogInformation("Refresh Plan Song Data: {refresh}", localAppConfig.RefreshPlanSongData);
            localAppConfig.RefreshSermonData = _config.GetValue<bool>(ServiceConstants.REFRESH_SERMON_DATA);
            _log.LogInformation("Refresh Sermon Data: {refresh}", localAppConfig.RefreshSermonData);
            localAppConfig.NumberOfDaysToRefreshGeneralSongData = _config.GetValue<int>(ServiceConstants.NUMBER_OF_DAYS_TO_REFRESH_GENERAL_SONG_DATA);
            _log.LogInformation("Number of days to refresh General Song Data: {numberOfDays}", localAppConfig.NumberOfDaysToRefreshGeneralSongData);
            localAppConfig.NumberOfDaysToRefreshPlanSongData = _config.GetValue<int>(ServiceConstants.NUMBER_OF_DAYS_TO_REFRESH_PLAN_SONG_DATA);
            _log.LogInformation("Number of days to refresh Plan Song Data: {numberOfDays}", localAppConfig.NumberOfDaysToRefreshPlanSongData);
            localAppConfig.NumberOfDaysToRefreshSermonData = _config.GetValue<int>(ServiceConstants.NUMBER_OF_DAYS_TO_REFRESH_SERMON_DATA);
            _log.LogInformation("Number of days to refresh Sermon Data: {numberOfDays}", localAppConfig.NumberOfDaysToRefreshSermonData);
            localAppConfig.NumberOfDaysToRefreshFutureData = _config.GetValue<int>(ServiceConstants.NUMBER_OF_DAYS_TO_REFRESH_FUTURE_DATA);
            _log.LogInformation("Number of days to refresh Future Data: {numberOfDays}", localAppConfig.NumberOfDaysToRefreshFutureData);
            return localAppConfig;
        }

        private PlanningCenterConfiguration LoadPlanningCenterConfiguration(AvailablePlansInformation plansInfo)
        {
            string rateLimit = _config.GetValue<string>(ServiceConstants.RATE_LIMIT_REQUEST);
            string ratePeriod = _config.GetValue<string>(ServiceConstants.RATE_PERIOD_REQUEST);
            PlanningCenterConfiguration configuration = new();
            var responseTask = plansInfo.Client.GetAsync(plansInfo.PlanUrlList[0]);
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
