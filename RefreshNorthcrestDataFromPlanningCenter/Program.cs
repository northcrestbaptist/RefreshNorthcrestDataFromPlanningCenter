using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RefreshNorthcrestDataFromPlanningCenter.Services;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic;

namespace RefreshNorthcrestDataFromPlanningCenter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            // Add code to write to email.  
            // Remove code that writes to the console.
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build()).Enrich
                .FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("C:\\logs\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Logger.Information("Application Starting");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IRetrievePlanningCenterDataService, RetrievePlanningCenterDataService>();
                    services.AddScoped<ITransformData, TransformData>();
                    services.AddScoped<INorthcrestLocalData, NorthcrestLocalData>();
                    services.AddScoped<IPlanningCenterInfo, PlanningCenterInfo>();
                    services.AddScoped<IGetSermonPlans, GetSermonPlans>();
                    services.AddScoped<IGetSermonPlanDetails, GetSermonPlanDetails>();
                })
                .UseSerilog()
                .Build();

            var svc = ActivatorUtilities.CreateInstance<RetrievePlanningCenterDataService>(host.Services);
            await svc.Run();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
