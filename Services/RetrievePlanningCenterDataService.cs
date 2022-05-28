using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{
    public class RetrievePlanningCenterDataService : IRetrievePlanningCenterDataService
    {
        
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        private readonly IConfiguration _config;
        int numberOfRequests = 0;
        PlanningCenterConfiguration planningCenterConfiguration;
        AvailablePlansInformation availablePlansInfo, sundayMorningPlans, sundayEveningPlans, specialServicePlans;
        string[] plansUrlList = new string[3] {
            ServiceConstants.SUNDAY_MORNING_SERVICE_PLANS_URL,
            ServiceConstants.SUNDAY_EVENING_SERVICE_PLANS_URL,
            ServiceConstants.SPECIAL_SERVICE_PLANS_URL};

        public RetrievePlanningCenterDataService(
            ILogger<RetrievePlanningCenterDataService> log, 
            IConfiguration config)
        {
            _log = log;
            _config = config;
            InitializePlanObjects();
        }
        public async Task Run()
        {
            _log.LogInformation("Starting data refresh.");
            using (HttpClient client = new())
            {
                ConfigureClient(client);
                GetPlanningCenterConfiguration(client);
    
                try
                {
                    for (int i=0; i < plansUrlList.Length; i++)
                    {
                        availablePlansInfo = GetAvailablePlansForServiceType(client, plansUrlList[i], i);
                        GetDetailsForAllPlansForServiceType(client, plansUrlList[i], availablePlansInfo);
                    }
                    
                }
                catch(Exception ex)
                {
                    _log.LogInformation("Encountered the following error: {error}", ex.Message);
                    _log.LogInformation("Error source: {source}", ex.Source);
                    _log.LogInformation("Stack Trace of error: {stackTrace}", ex.StackTrace);
                }

            }
            Console.Write("Date refresh completed successfully. Press any key to stop the program.");
            Console.ReadLine();
        }

        private void InitializePlanObjects()
        {
            planningCenterConfiguration = new PlanningCenterConfiguration();
            sundayMorningPlans = new AvailablePlansInformation();
            sundayEveningPlans = new AvailablePlansInformation();
            specialServicePlans = new AvailablePlansInformation();
        }

        private void ConfigureClient(HttpClient client)
        {
            string appID = _config.GetValue<string>(ServiceConstants.APP_ID);
            string secret = _config.GetValue<string>(ServiceConstants.SECRET);
            var authenticationString = $"{appID}:{secret}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ServiceConstants.BASIC, base64EncodedAuthenticationString);
        }

        private void GetPlanningCenterConfiguration(HttpClient client)
        {
            var responseTask = client.GetAsync(ServiceConstants.SUNDAY_MORNING_SERVICE_PLANS_URL);
            numberOfRequests++;
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                int parseResult;
                bool parseSuccess;
                parseSuccess = int.TryParse(result.Headers.GetValues(ServiceConstants.RATE_LIMIT_REQUEST).FirstOrDefault(), out parseResult);
                planningCenterConfiguration.RateLimit = parseSuccess ? parseResult : 100;
                _log.LogInformation("Retrieved rate limit. {rateLimit}", planningCenterConfiguration.RateLimit);
                parseSuccess = int.TryParse(result.Headers.GetValues(ServiceConstants.RATE_PERIOD_REQUEST).FirstOrDefault(), out parseResult);
                planningCenterConfiguration.RatePeriod = parseSuccess ? parseResult : 20;
                _log.LogInformation("Retrieved rate period. {ratePeriod}", planningCenterConfiguration.RatePeriod);
            }
            else
            {
                _log.LogError("There was an error accessing the website initially: {error}", result.ReasonPhrase);
            }
        }

        private AvailablePlansInformation GetAvailablePlansForServiceType(HttpClient client, string planUrl, int serviceTypeIndex)
        {
            string[] serviceType= new string[3] { ServiceConstants.SUNDAY_MORNING_SERVICE, ServiceConstants.SUNDAY_EVENING_SERVICE, ServiceConstants.SPECIAL_SERVICE };
            AvailablePlansInformation plansInformation = new();
            var responseTask = client.GetAsync(planUrl);
            numberOfRequests++;
            responseTask.Wait();

            var result = responseTask.Result;
            var readPlanForTotalCountTask = result.Content.ReadAsStringAsync();
            readPlanForTotalCountTask.Wait();
            string firstResults = readPlanForTotalCountTask.Result;
            plansInformation.CurrentRetrievedPlans = JsonConvert.DeserializeObject<Plans>(firstResults);
            plansInformation.TotalRecordsAvailable = plansInformation.CurrentRetrievedPlans.meta.total_count;
            plansInformation.RemainingRecords = plansInformation.CurrentRetrievedPlans.meta.total_count;
            _log.LogInformation("Obtained information for {serviceType} plans.", serviceType[serviceTypeIndex]);
            _log.LogInformation("Total plans available. {totalPlansAvailable}", plansInformation.CurrentRetrievedPlans.meta.total_count);
            //Console.Write("Press any key to continue.");
            //Console.ReadLine();
            return plansInformation;
        }

        private void GetDetailsForAllPlansForServiceType(HttpClient client, string plansBaseUrl, AvailablePlansInformation plansInformation)
        {
            while (plansInformation.RemainingRecords > 0)
            {
                GetNextPageOfPlans(client, plansBaseUrl, plansInformation);
                foreach (Plan plan in plansInformation.CurrentRetrievedPlans.data)
                {
                    if (plan.attributes.sort_date < DateTime.Now)
                    {
                        GetItemsForPlan(client, plansBaseUrl, plansInformation, plan);
                        foreach (Item item in plansInformation.CurrentRetrievedItems.data)
                        {
                            if (item.attributes.title.ToLower().Contains(ServiceConstants.SERMON))
                            {
                                GetSermonDetails(client, plansBaseUrl, plansInformation, plan, item);
                            }
                        }
                    }
                }
                plansInformation.RemainingRecords -= planningCenterConfiguration.RateLimit;
                plansInformation.OffSet += planningCenterConfiguration.RateLimit;
            }
        }

        private void GetNextPageOfPlans(HttpClient client, string plansBaseUrl, AvailablePlansInformation plansInformation)
        {
            string getPlansByPage =
                    $"{plansBaseUrl}?offset={plansInformation.OffSet}&per_page={planningCenterConfiguration.RateLimit}";
            _log.LogInformation("Remaining Planning Center plans: {remainingRecords}", plansInformation.RemainingRecords);
            _log.LogInformation("Current offset required to get next page: {offSet}", plansInformation.OffSet);
            _log.LogInformation("Retrieving next page of plans...");
            numberOfRequests++;
            var responseTask = client.GetAsync(getPlansByPage);
            numberOfRequests++;
            responseTask.Wait();
            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            readTask.Wait();
            string planningCenterResults = readTask.Result;
            plansInformation.CurrentRetrievedPlans = JsonConvert.DeserializeObject<Plans>(planningCenterResults);
        }

        private void GetItemsForPlan(HttpClient client, string plansBaseUrl, AvailablePlansInformation plansInformation, Plan plan)
        {
            _log.LogInformation("Retrieving items for: {planID}", plan.id);
            string getSpecificPlan = $"{plansBaseUrl}/{plan.id}/items";
            var responseTask = client.GetAsync(getSpecificPlan);
            numberOfRequests++;
            responseTask.Wait();

            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            if (numberOfRequests > planningCenterConfiguration.RateLimit - 5)
            {
                _log.LogInformation("Pausing program execution to adhere to Planning Center web api request limits.");
                Thread.Sleep(1000 * planningCenterConfiguration.RatePeriod);
                numberOfRequests = 0;
                readTask.Wait();
            }
            else
            {
                readTask.Wait();
            }

            string planItemResults = readTask.Result;

            plansInformation.CurrentRetrievedItems = JsonConvert.DeserializeObject<Items>(planItemResults);
        }

        private void GetSermonDetails(HttpClient client, string plansBaseUrl, AvailablePlansInformation plansInformation, Plan plan, Item item)
        {
            _log.LogInformation("Date/Time: {serviceDate}", plan.attributes.sort_date);
            _log.LogInformation("Title: {sermonTitle}", item.attributes.title);
            _log.LogInformation("Description: {sermonDescription}", item.attributes.description);
            string getSpecificPlan = $"{plansBaseUrl}/{plan.id}/items";
            string getItemNotesUrl = $"{getSpecificPlan}/{item.id}/item_notes";
            var responseTask = client.GetAsync(getItemNotesUrl);
            numberOfRequests++;
            responseTask.Wait();

            var result = responseTask.Result;


            var readTask = result.Content.ReadAsStringAsync();
            readTask.Wait();

            string itemNotesResults = readTask.Result;
            plansInformation.CurrentRetrievedItemNotes = JsonConvert.DeserializeObject<ItemNotes>(itemNotesResults);
            if (plansInformation.CurrentRetrievedItemNotes.data != null)
            {
                foreach (ItemNote note in plansInformation.CurrentRetrievedItemNotes.data) // foreach 
                {
                    _log.LogInformation("Speaker: {itemNotesName}", note.attributes.content);
                }
            }
        }
    }
}
