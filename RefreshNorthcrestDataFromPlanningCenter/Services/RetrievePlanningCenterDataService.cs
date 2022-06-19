using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{

    public class RetrievePlanningCenterDataService : IRetrievePlanningCenterDataService
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        private readonly IConfiguration _config;
        private readonly INorthcrestLocalData _northcrestLocalData;
        private readonly IPlanningCenterInfo _planningCenterInfo;
        private readonly IGetSermonPlans _getSermonPlans;
        private readonly IGetSermonPlanDetails _getSermonPlanDetails;
        private string[] _plansUrlList = new string[3];
        private readonly string[] _serviceType = new string[3]
        {
            ServiceConstants.SUNDAY_MORNING_SERVICE,
            ServiceConstants.SUNDAY_EVENING_SERVICE,
            ServiceConstants.SPECIAL_SERVICE
        }; 
        private AvailablePlansInformation _availablePlansInfo;

        public RetrievePlanningCenterDataService(
            ILogger<RetrievePlanningCenterDataService> log, 
            IConfiguration config,
            INorthcrestLocalData northcrestLocalData,
            IPlanningCenterInfo planningCenterInfo,
            IGetSermonPlans getSermonPlans,
            IGetSermonPlanDetails getSermonPlanDetails
            )
        {
            _log = log;
            _config = config;
            _northcrestLocalData = northcrestLocalData;
            _planningCenterInfo = planningCenterInfo;
            _getSermonPlans = getSermonPlans;
            _getSermonPlanDetails = getSermonPlanDetails;
            _availablePlansInfo = new AvailablePlansInformation();
            LoadUrls();
        }
        public async Task Run()
        {
            _log.LogInformation("Starting data refresh...");
            using (_availablePlansInfo.Client = new())
            {
                try
                {
                    _availablePlansInfo.MostRecentSermonInNorthcrestDatabase = _northcrestLocalData.GetLatestSermonDateTime();
                    ConfigureClient(_availablePlansInfo.Client);
                    _availablePlansInfo.Configuration = _planningCenterInfo.GetPlanningCenterConfiguration(_availablePlansInfo.Client);
                    _availablePlansInfo.NumberOfRequests++;
                    _log.LogInformation("Searching for service updates...");
                    for (int i = 0; i < _plansUrlList.Length; i++)
                    {
                        _availablePlansInfo.Url = _plansUrlList[i];
                        _availablePlansInfo.Type = _serviceType[i];
                        _getSermonPlans.GetAvailablePlansForServiceType(_availablePlansInfo);
                        _availablePlansInfo.NumberOfRequests++;
                        _getSermonPlanDetails.GetDetailsForAllPlansForServiceType(_availablePlansInfo);
                    }

                    _northcrestLocalData.AddSermonsToDatabase(_availablePlansInfo.Sermons);


                }
                catch(Exception ex)
                {
                    _log.LogInformation("Encountered the following error: {error}", ex.Message);
                    _log.LogInformation("Error source: {source}", ex.Source);
                    _log.LogInformation("Stack Trace of error: {stackTrace}", ex.StackTrace);
                }

            }
            _log.LogInformation("Date refresh completed successfully.");
            
        }

        private void ConfigureClient(HttpClient client)
        {
            string appID = _config.GetValue<string>(ServiceConstants.APP_ID);
            string secret = _config.GetValue<string>(ServiceConstants.SECRET);
            string basic = _config.GetValue<string>(ServiceConstants.BASIC);
            var authenticationString = $"{appID}:{secret}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(basic, base64EncodedAuthenticationString);
        }

        private void LoadUrls()
        {
            string sundayMonrningPlansUrl = _config.GetValue<string>(ServiceConstants.SUNDAY_MORNING_PLANS_URL_CONFIG);
            string sundayEveningPlansUrl = _config.GetValue<string>(ServiceConstants.SUNDAY_EVENING_PLANS_URL_CONFIG);
            string specialPlansUrl = _config.GetValue<string>(ServiceConstants.SPECIAL_PLANS_URL_CONFIG);
            _plansUrlList[0] = sundayMonrningPlansUrl;
            _plansUrlList[1] = sundayEveningPlansUrl;
            _plansUrlList[2] = specialPlansUrl;
        }
    }
}
