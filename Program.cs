using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RefreshNorthcrestDataFromPlanningCenter.Models;
using RefreshNorthcrestDataFromPlanningCenter.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Services;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using Serilog;
using System;
using System.IO;

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
                    services.AddTransient<IRetrievePlanningCenterDataService, RetrievePlanningCenterDataService>();
                })
                .UseSerilog()
                .Build();

            var svc = ActivatorUtilities.CreateInstance<RetrievePlanningCenterDataService>(host.Services);
            svc.Run();
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
}
