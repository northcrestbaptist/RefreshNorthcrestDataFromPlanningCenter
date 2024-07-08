using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromFellowshipOne.Common.Constants;
using RefreshNorthcrestDataFromFellowshipOne.Models.Local;
using RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne;
using RefreshNorthcrestDataFromFellowshipOne.Services.Interfaces;
using System.Net.Http.Headers;

namespace RefreshNorthcrestDataFromFellowshipOne.Services
{
    public class NorthcrestConfigurationService : INorthcrestConfigurationService
    {
        private readonly ILogger<RetrieveFellowshipOneDataService> _log;
        private readonly IConfiguration _config;

        public NorthcrestConfigurationService(ILogger<RetrieveFellowshipOneDataService> log,
            IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public void SetConfiguration(FellowshipOneInformation fellowshipOneInfo)
        {
            fellowshipOneInfo.RefreshAppConfig = LoadLocalAppConfigurations();
            LoadClientConfiguration(fellowshipOneInfo.Client);
            LoadUris(fellowshipOneInfo);
        }

        public bool IsAnythingConfiguredToRefresh(FellowshipOneInformation fellowshipOneInfo)
        {
            return fellowshipOneInfo.RefreshAppConfig.RefreshPersonnelData;
        }

        public void LogLocalAppConfigurationInstructions()
        {
            _log.LogInformation("{configInstructions}", "Configuration Instructions:");
            _log.LogInformation("The app refresh configuration can be changed in: {configLocation}", "C:\\RefreshNorthcrestDatabase\\FellowshipOne\\appsettings.json.");
            _log.LogInformation("The following values can be set to {true} or {false} to enable each refresh: " +
                "{RefreshPersonnelData}", true, false, ServiceConstants.APICONFIG_REFRESH_PERSONNEL_DATA);
       }

        private void LoadClientConfiguration(HttpClient client)
        {
            string? baseUrl = _config.GetValue<string>(ServiceConstants.URI_BASE_URL);
            string? appId = _config.GetValue<string>(ServiceConstants.APICONFIG_APP_ID);
            string? secret = _config.GetValue<string>(ServiceConstants.APICONFIG_SECRET);
            string? tokenType = _config.GetValue<string>(ServiceConstants.APICONFIG_TOKEN_TYPE);
            string? tokenValue = _config.GetValue<string>(ServiceConstants.APICONFIG_TOKEN_VALUE);
            string? mediaType = _config.GetValue<string>(ServiceConstants.APICONFIG_MEDIA_TYPE);

            if (!String.IsNullOrEmpty(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }
            else
            {
                _log.LogError("Unable to acquire baseUrl from the app configuration file for accessing the Fellowship One web serice.");
                throw new ArgumentNullException(nameof(baseUrl));
            }


            if (!String.IsNullOrEmpty(appId) && !String.IsNullOrEmpty(secret))
            {
                client.DefaultRequestHeaders.Add(appId, secret);
            }
            else
            {
                _log.LogError("Unable to acquire appID and secrent from the app configuration file for accessing the Fellowship One web serice.");
                if (!String.IsNullOrEmpty(appId)) throw new ArgumentNullException(nameof(appId));
                else throw new ArgumentNullException(nameof(secret));
            }

            if (!String.IsNullOrEmpty(tokenType) && !String.IsNullOrEmpty(tokenValue))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, tokenValue);
            }
            else
            {
                _log.LogError("Unable to acquire tokenType and tokenValue from the app configuration file for accessing the Fellowship One web serice.");
                if (!String.IsNullOrEmpty(tokenType)) throw new ArgumentNullException(nameof(tokenType));
                else throw new ArgumentNullException(nameof(tokenValue));
            }

            if (!String.IsNullOrEmpty(mediaType))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));//ACCEPT header
            }
            else
            {
                _log.LogError("Unable to acquire mediaType from the app configuration file for accessing the Fellowship One web serice.");
                throw new ArgumentNullException(nameof(mediaType));
            }
        }

        private void LoadUris(FellowshipOneInformation fellowshipOneInfo)
        {
            string? getAllPeopleApi = _config.GetValue<string>(ServiceConstants.URI_PEOPLE_ALL_WITH_ADDRESSES_COMMUNICATIONS_ATTRIBUTES);
            string? getPersonBackgroundInvestigationApi = _config.GetValue<string>(ServiceConstants.URI_REQUIREMENTS_SEARCH);

            if (!String.IsNullOrEmpty(getAllPeopleApi))
            {
                fellowshipOneInfo.GetAllPeopleApi = getAllPeopleApi;
            }
            else
            {
                _log.LogError("Unable to acquire getAllPeopleApi from the app configuration file for accessing the Fellowship One web serice.");
                throw new ArgumentNullException(nameof(getAllPeopleApi));
            }
            if (!String.IsNullOrEmpty(getPersonBackgroundInvestigationApi))
            {
                fellowshipOneInfo.GetPersonBackgroundInvestigationInfoApiTemplate = getPersonBackgroundInvestigationApi;
            }
            else
            {
                _log.LogError("Unable to acquire getPersonBackgroundInvestigationApi from the app configuration file for accessing the Fellowship One web serice.");
                throw new ArgumentNullException(nameof(getPersonBackgroundInvestigationApi));
            }
        }

        private LocalAppConfiguration LoadLocalAppConfigurations()
        {
            LocalAppConfiguration localAppConfig = new LocalAppConfiguration();
            _log.LogInformation("Retieving local refresh configurations...");
            localAppConfig.RefreshPersonnelData = _config.GetValue<bool>(ServiceConstants.APICONFIG_REFRESH_PERSONNEL_DATA);
            _log.LogInformation("Refresh Personnel Data: {refresh}", localAppConfig.RefreshPersonnelData);
            return localAppConfig;
        }
    }
}
