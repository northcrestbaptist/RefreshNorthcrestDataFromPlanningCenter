using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.Models;
using RefreshNorthcrestDataFromPlanningCenter.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{
    public class RetrievePlanningCenterDataService : IRetrievePlanningCenterDataService
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        private readonly IConfiguration _config;
        int rateLimit;
        int numberOfRequests;
        int totalRecordsAvailable;
        int remainingRecords;
        int offSet = 0;
        int ratePeriod;
        int parseResult;
        bool parseSuccess;
        Plans currentRetrievedPlans;
        Plans allRetrievedPlansWithSermonText;
        Plan currentPlan;

        public RetrievePlanningCenterDataService(
            ILogger<RetrievePlanningCenterDataService> log, 
            IConfiguration config)
        {
            _log = log;
            _config = config;
        }
        public async void Run()
        {
            //Console.WriteLine("Press any key to begin the data retieval process.");
            //Console.ReadLine();
            _log.LogInformation("Starting data refresh.");

            //string planningCenterWebsite = "https://api.planningcenteronline.com/services/v2/service_types/107395/plans/2450442/items";
            string getSpecificSundayPlanBaseSite = "https://api.planningcenteronline.com/services/v2/service_types/107395/plans";
            string getSpecificSundayPlan;
            string getAllSundayMorningPlansbaseSite = "https://api.planningcenteronline.com/services/v2/service_types/107395/plans";
            string getAllSundayMorningPlansByPage;
            // https://api.planningcenteronline.com/services/v2/service_types/107395/plans = Sunday morning plans
            // https://api.planningcenteronline.com/services/v2/service_types/107396/plans = Sunday evening plans
            // https://api.planningcenteronline.com/services/v2/service_types/107397/plans = Special Service plans
            // How to navigate pages - current limit 100 per page per 20 seconds.
            // https://api.planningcenteronline.com/services/v2/service_types/107397/plans?offset=100&per_page=100
            string appID = _config.GetValue<string>("appID");
            string secret = _config.GetValue<string>("secret");

            using (var client = new HttpClient())
            {
                //string appID = "f1dd2c80d2d57e65ba2c282ca1fbc1af59d039cf40e4c41d06022890ba066340";
                //string secret = "c1fc1ea67b68524b6124423ee95195f361ed51df226770f03bd75d0a7bbe488c";
                var authenticationString = $"{appID}:{secret}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));


                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);


                var responseTask = client.GetAsync(getAllSundayMorningPlansbaseSite);
                numberOfRequests++;
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success!");
                    
                    parseSuccess = int.TryParse(result.Headers.GetValues("X-PCO-API-Request-Rate-Limit").FirstOrDefault(), out parseResult);
                    rateLimit = parseSuccess ? parseResult : 100;
                    _log.LogInformation("Retrieved rate limit. {rateLimit}", rateLimit);
                    parseSuccess = int.TryParse(result.Headers.GetValues("X-PCO-API-Request-Rate-Period").FirstOrDefault(), out parseResult);
                    ratePeriod = parseSuccess ? parseResult : 20;
                    _log.LogInformation("Retrieved rate period. {ratePeriod}", ratePeriod);
                    var readPlanForTotalCounhTask = result.Content.ReadAsStringAsync();
                    readPlanForTotalCounhTask.Wait();

                    //var header = "Header: " + result.Headers.ToString();
                    string firstResults = readPlanForTotalCounhTask.Result;

                    currentRetrievedPlans = JsonConvert.DeserializeObject<Plans>(firstResults);
                    totalRecordsAvailable = currentRetrievedPlans.meta.total_count;
                    remainingRecords = currentRetrievedPlans.meta.total_count;
                    _log.LogInformation("Total plans available. {totalPlansAvailable}", currentRetrievedPlans.meta.total_count);
                }
                else
                {
                    _log.LogError("There was an error accessing the website initially: {error}", result.ReasonPhrase);
                }
                allRetrievedPlansWithSermonText = new Plans();
                List<int> planIDs = new List<int>();
                while (remainingRecords > 0)
                {
                    //if(remainingRecords > rateLimit - 5)
                    //{
                    //    await Task.Delay(ratePeriod * 1000);
                    //    numberOfRequests = 0;
                    //}
                    //else
                    //{
                        //await Task.Delay(1000);
                        getAllSundayMorningPlansByPage = $"{getAllSundayMorningPlansbaseSite}?offset={offSet}&per_page={rateLimit}";
                        _log.LogInformation("Remaining Planning Center plans: {remainingRecords}", remainingRecords);
                        _log.LogInformation("Current offset: {offSet}", offSet);
                        _log.LogInformation("Total Planning Center plans retrieved: {currentReceivedRecordCoun}", planIDs.Count);
                        _log.LogInformation("calling this website now:{url}", getAllSundayMorningPlansByPage);
                        numberOfRequests++;
                        responseTask = client.GetAsync(getAllSundayMorningPlansByPage);
                        numberOfRequests++;
                        responseTask.Wait();

                        result = responseTask.Result;

                        if (result.IsSuccessStatusCode)
                        {

                            var readTask = result.Content.ReadAsStringAsync();
                            readTask.Wait();

                            //var header = "Header: " + result.Headers.ToString();
                            string planningCenterResults = readTask.Result;

                            currentRetrievedPlans = JsonConvert.DeserializeObject<Plans>(planningCenterResults);

                            _log.LogInformation("Total plans available. {totalPlansAvailable}", currentRetrievedPlans.meta.total_count);
                            foreach (Plan plan in currentRetrievedPlans.data)
                            {
                                planIDs.Add(plan.id);
                                _log.LogInformation("Retrieving items for: {planID}", plan.id);
                                getSpecificSundayPlan = $"{getSpecificSundayPlanBaseSite}/{plan.id}/items";
                                responseTask = client.GetAsync(getSpecificSundayPlan);
                                numberOfRequests++;
                                responseTask.Wait();

                                result = responseTask.Result;

                                if (result.IsSuccessStatusCode)
                                {
                                    var readItemsTask = result.Content.ReadAsStringAsync();
                                    if (numberOfRequests > rateLimit - 5)
                                    {
                                        _log.LogInformation("Pausing programe execution to adhere to Planning Center web api request limits.");
                                        Thread.Sleep(1000 * ratePeriod);     
                                        numberOfRequests = 0;
                                        readTask.Wait();                                        
                                    }
                                    else
                                    {
                                        readTask.Wait();
                                    }
                                    

                                    //var header = "Header: " + result.Headers.ToString();
                                    string planItemResults = readItemsTask.Result;

                                    Items items = JsonConvert.DeserializeObject<Items>(planItemResults);
                                    foreach (Item item in items.data)
                                    {
                                        if (item.attributes.title.ToLower().Contains("sermon"))
                                        {
                                            _log.LogInformation("Service Date: {serviceDate}", plan.attributes.sort_date);
                                            //_log.LogInformation("Series Title: {seriesTitle}", plan.attributes.series_title);
                                            //_log.LogInformation("Number of Items from Plan: {numberOfItems}", plan.attributes.items_count);
                                            _log.LogInformation("Sermon Title: {sermonTitle}", item.attributes.title);
                                            //_log.LogInformation("Sermon Description: {sermonDescription}", item.attributes.description);
                                        }
                                        else
                                        {
                                            //_log.LogInformation("No item was found with the word: sermon.");
                                        }
                                    }

                                }
                                else
                                {
                                    _log.LogError("There was an error accessing the website retrieving the data {error}", result.ReasonPhrase);
                                }
                                //if (plan.attributes.multi_day)
                                //{
                                //    planIDs.Add(plan.id);
                                //}

                            }
                            remainingRecords = remainingRecords - rateLimit;
                            offSet = offSet + rateLimit;
                        }
                        else
                        {
                            _log.LogError("There was an error accessing the website retrieving the data {error}", result.ReasonPhrase);
                            break;

                        }
                        //break;
                    //}

                }

                _log.LogInformation("Complete List of Plan IDs:");
                foreach(int number in planIDs)
                {
                    Console.Write($"{number} ");
                }

                //getSpecificSundayPlan = $"{getSpecificSundayPlanBaseSite}/{planIDs.ElementAt(580)}/items";
                //responseTask = client.GetAsync(getSpecificSundayPlan);

                //responseTask.Wait();

                //result = responseTask.Result;

                //if (result.IsSuccessStatusCode)
                //{
                //    var readTask = result.Content.ReadAsStringAsync();
                //    readTask.Wait();

                //    //var header = "Header: " + result.Headers.ToString();
                //    string planningCenterResults = readTask.Result;
                //    _log.LogInformation("Retrieved this plan: {planningCenterResults}", planningCenterResults);
                //    //currentPlan = JsonConvert.DeserializeObject<Plan>(planningCenterResults);

                //}
                //else
                //{
                //    _log.LogError("There was an error accessing the website retrieving the data {error}", result.ReasonPhrase);
                //}


            }
            Console.ReadLine();
        }

        

    }
}
