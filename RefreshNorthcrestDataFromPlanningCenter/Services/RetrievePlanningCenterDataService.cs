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
        private readonly INorthcrestConfigurationService _northcrestConfigurationService;
        private readonly IDatabaseUpdateService _databaseUpdateService;
        private readonly IRetrievePlanDataService _retrievePlanDataService;
        private AvailablePlansInformation _availablePlansInfo;

        public RetrievePlanningCenterDataService(
            ILogger<RetrievePlanningCenterDataService> log, 
            INorthcrestConfigurationService northcrestConfigurationService,
            IDatabaseUpdateService databaseUpdateService,
            IRetrievePlanDataService retrievePlanDataService            )
        {
            _log = log;
            _northcrestConfigurationService = northcrestConfigurationService;
            _databaseUpdateService = databaseUpdateService;
            _retrievePlanDataService = retrievePlanDataService;
            _availablePlansInfo = new AvailablePlansInformation();
        }
        public async Task RunAsync()
        {
            _log.LogInformation("Starting data refresh...");
            using (_availablePlansInfo.Client = new())
            {
                try
                {
                    _northcrestConfigurationService.LogLocalAppConfigurationInstructions();
                    _log.LogInformation("Retrieving and setting Northcrest and Planning Center configurations...");
                    _northcrestConfigurationService.SetConfiguration(_availablePlansInfo);
                    bool refreshSomething = _northcrestConfigurationService.IsAnythingConfiguredToRefresh(_availablePlansInfo);
                    if (refreshSomething)
                    {
                        _databaseUpdateService.DeleteLocalDataToBeRefreshed(_availablePlansInfo);
                        _log.LogInformation("Retrieving sermon and song data from Planning Center according to current configurations...");
                        await GetPlanDataFromPlanningCenterAsync();
                        _databaseUpdateService.RefreshDataThatWasRetrievedFromPlanningCenter(_availablePlansInfo);
                    }
                    else
                    {
                        _log.LogInformation("None of the data is configured to refresh.");
                        _northcrestConfigurationService.LogLocalAppConfigurationInstructions();
                    }
                }
                catch(Exception ex)
                {
                    _log.LogInformation("Encountered the following error: {error}", ex.Message);
                    _log.LogInformation("Error source: {source}", ex.Source);
                    _log.LogInformation("Stack Trace of error: {stackTrace}", ex.StackTrace);
                }

            }
            _log.LogInformation("A total of {genSongsCount} songs were refreshed to the GeneralSongs table during a search through {plansCount} service plans.",
                _availablePlansInfo.NumberOfGeneralSongsRefreshed, _availablePlansInfo.NumberofPlansUsedToRefreshGeneralSongs);
            
            
        }

        private async Task GetPlanDataFromPlanningCenterAsync()
        {
            for (int i = 0; i < _availablePlansInfo.PlanUrlList.Length; i++)
            {
                _availablePlansInfo.CurrentUrl = _availablePlansInfo.PlanUrlList[i];
                _availablePlansInfo.CurrentPlanType = _availablePlansInfo.PlanTypeList[i];
                _retrievePlanDataService.GetAvailablePlansForServiceType(_availablePlansInfo);
                _availablePlansInfo.NumberOfRequests++;
                await _retrievePlanDataService.GetDetailsForAllPlansForServiceTypeAsync(_availablePlansInfo);
            }
        }
    }
}
