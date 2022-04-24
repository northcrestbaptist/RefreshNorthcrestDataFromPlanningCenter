using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.Models;
using RefreshNorthcrestDataFromPlanningCenter.Models.Interfaces;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RefreshNorthcrestDataFromPlanningCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build()).Enrich
                .FromLogContext().
                WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application Starting");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {

                })
                .UseSerilog()
                .Build();
            
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }

        //private PersonIdentityResponseItem GetPersonFromMM_DMDC_IWS(string edipi, string indNMilEmplCd = null, string dodServiceComponentCode = null)
        //{
        //    IDmdcRequest dmdcRequest = _dmdcRequestFactory.CreateDmdcRequest(edipi, indNMilEmplCd, dodServiceComponentCode);

        //    string mmWebServicesDMDCIWSwebsite =
        //        NetworkConstants.HTTPS + NetworkHelper.WebServicesDMDCIWSHostName +
        //        NetworkConstants.WEB_SERVICES_DMDC_IWS_URI + NetworkConstants.WEB_SERVICES_DMDC_IWS_GET_PERSON_BY;

        //    string jsonDmdcRequest = System.Text.Json.JsonSerializer.Serialize(dmdcRequest);
        //    X509Certificate2 certificate = GetCertificate();
        //    HttpClientHandler handler = new();
        //    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        //    handler.ClientCertificates.Add(certificate);

        //    HttpClient request = new(handler);
        //    StringContent content = new(jsonDmdcRequest);
        //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        //    var response = request.PostAsync(mmWebServicesDMDCIWSwebsite, content).Result;
        //    string responseString = response.Content.ReadAsStringAsync().Result;
        //    PersonIdentityResponseItem results = Newtonsoft.Json.JsonConvert.DeserializeObject<PersonIdentityResponseItem>(responseString);

        //    return results;
        //}
    }
    public class RetrievePlanningCenterDataService
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        private readonly IConfiguration _config;

        public RetrievePlanningCenterDataService(ILogger<RetrievePlanningCenterDataService> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }
        public void Run()
        {
            //Console.WriteLine("Press any key to begin the data retieval process.");
            //Console.ReadLine();
            _log.LogInformation("Starting data refresh.");

            //string planningCenterWebsite = "https://api.planningcenteronline.com/services/v2/service_types/107395/plans/2450442/items";
            string planningCenterWebsite = "https://api.planningcenteronline.com/services/v2/service_types/107395/plans";
            // https://api.planningcenteronline.com/services/v2/service_types/107395/plans = Sunday morning plans
            // https://api.planningcenteronline.com/services/v2/service_types/107396/plans = Sunday evening plans
            // https://api.planningcenteronline.com/services/v2/service_types/107397/plans = Special Service plans
            // How to navigate pages - current limit 100 per page per 20 seconds.
            // https://api.planningcenteronline.com/services/v2/service_types/107397/plans?offset=100&per_page=100


            using (var client = new HttpClient())
            {
                string appID = "f1dd2c80d2d57e65ba2c282ca1fbc1af59d039cf40e4c41d06022890ba066340";
                string secret = "c1fc1ea67b68524b6124423ee95195f361ed51df226770f03bd75d0a7bbe488c";
                var authenticationString = $"{appID}:{secret}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));


                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);


                var responseTask = client.GetAsync(planningCenterWebsite);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Success!");
                    int rateLimit;
                    int ratePeriod;
                    int parseResult;
                    bool parseSuccess;
                    parseSuccess = int.TryParse(result.Headers.GetValues("X-PCO-API-Request-Rate-Limit").FirstOrDefault(), out parseResult);
                    rateLimit = parseSuccess ? parseResult : 100;
                    _log.LogInformation("Retrieved rate limit. { rateLimit }", rateLimit);
                    parseSuccess = int.TryParse(result.Headers.GetValues("X-PCO-API-Request-Rate-Period").FirstOrDefault(), out parseResult);
                    ratePeriod = parseSuccess ? parseResult : 20;
                    _log.LogInformation("Retrieved rate period. { ratePeriod }", ratePeriod);

                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    //var header = "Header: " + result.Headers.ToString();
                    string planningCenterResults = readTask.Result;


                    // To implement deserialization using the interfaces, see this solution.  It's not necessary, so
                    // I skipped it.  https://stackoverflow.com/questions/50613560/how-to-deserialize-interfaces-with-newtonsoft-json-net
                    
                    Plans plans = JsonConvert.DeserializeObject<Plans>(planningCenterResults);
                    _log.LogInformation("Total plans available. { totalPlansAvailable }", plans.meta.total_count);
                    foreach (Plan plan in plans.data)
                    {
                        _log.LogInformation("Plan ID. { planID }", plan.id);
                    }
                    

                    Console.WriteLine(planningCenterResults);
                }
                else
                {
                    Console.WriteLine("Failure!");
                }
            }
            Console.ReadLine();
        }

    }
}
