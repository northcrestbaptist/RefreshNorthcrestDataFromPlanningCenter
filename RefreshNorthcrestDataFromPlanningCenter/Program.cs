using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Services;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using Serilog;

using Serilog.Sinks.Email;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            
            try
            {
                BuildConfig(builder);
                IConfiguration config = builder.Build();
                string fromEmail = config.GetValue<string>(ServiceConstants.EMAIL_FROM);
                string toMediaEmail = config.GetValue<string>(ServiceConstants.EMAIL_TO_MEDIA);
                string toLauraEmail = config.GetValue<string>(ServiceConstants.EMAIL_TO_LAURA);
                string toRichieEmail = config.GetValue<string>(ServiceConstants.EMAIL_TO_RICHIE);
                string emailServerAddress = config.GetValue<string>(ServiceConstants.EMAIL_SERVER_ADDRESS);
                int emailServerPort = config.GetValue<int>(ServiceConstants.EMAIL_SERVER_PORT);
                bool emailServerSsl = config.GetValue<bool>(ServiceConstants.EMAIL_SERVER_SSL);
                string emailServerLogonUserId = config.GetValue<string>(ServiceConstants.EMAIL_SERVER_USERID);
                string emailServerLogonPassword = config.GetValue<string>(ServiceConstants.EMAIL_SERVER_PASSWORD);

                var mediaEmailConnectionInfo = new EmailConnectionInfo
                {
                    FromEmail = fromEmail,
                    ToEmail = toMediaEmail,
                    MailServer = emailServerAddress,
                    NetworkCredentials = new NetworkCredential
                    {
                        UserName = emailServerLogonUserId,
                        Password = emailServerLogonPassword
                    },
                    EnableSsl = emailServerSsl,
                    EmailSubject = ServiceConstants.EMAIL_SUBJECT,
                    Port = emailServerPort
                };

                var lauraEmailConnectionInfo = new EmailConnectionInfo
                {
                    FromEmail = fromEmail,
                    ToEmail = toLauraEmail,
                    MailServer = emailServerAddress,
                    NetworkCredentials = new NetworkCredential
                    {
                        UserName = emailServerLogonUserId,
                        Password = emailServerLogonPassword
                    },
                    EnableSsl = emailServerSsl,
                    EmailSubject = ServiceConstants.EMAIL_SUBJECT,
                    Port = emailServerPort
                };

                var richieEmailConnectionInfo = new EmailConnectionInfo
                {
                    FromEmail = fromEmail,
                    ToEmail = toRichieEmail,
                    MailServer = emailServerAddress,
                    NetworkCredentials = new NetworkCredential
                    {
                        UserName = emailServerLogonUserId,
                        Password = emailServerLogonPassword
                    },
                    EnableSsl = emailServerSsl,
                    EmailSubject = ServiceConstants.EMAIL_SUBJECT,
                    Port = emailServerPort
                };

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Build())
                    .Enrich
                    .FromLogContext()
                    //.WriteTo.Console()
                    // Once you need Network Credentials, the WriteTo.Email configuration cannot be included 
                    // in the appconfig.
                    //.WriteTo.Email(mediaEmailConnectionInfo, batchPostingLimit: 100,
                    // restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
                    //.WriteTo.Email(lauraEmailConnectionInfo, batchPostingLimit: 100,
                    // restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
                    .WriteTo.Email(richieEmailConnectionInfo, batchPostingLimit: 100,
                     restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
                    .CreateLogger();
                Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

                Log.Logger.Information("Application Starting...");
                Log.Logger.Information("The Northcrest Data Refresh app will execute in 20 seconds.");
                Log.Logger.Information("Please close this app immediately if you started by mistake.");
                Log.Logger.Information("Once the 20 seconds elapses and the processing begins, you must allow the app to run to its conclusion.");
                Thread.Sleep(10000);
                Log.Logger.Information("10 second count down beginning...");
                for (int i = 10; i > -1; i--)
                {
                    Thread.Sleep(1000);
                    Log.Logger.Information("{countdown}...", i);
                }
                Log.Logger.Information("Application Liftoff!!!");
                Log.Logger.Information("You must not stop the application at this point!");
                Log.Logger.Information("You must allow the execution to run to its completion!");
                Thread.Sleep(3000);
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                var host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        services.AddTransient<IRetrievePlanningCenterDataService, RetrievePlanningCenterDataService>();
                        services.AddScoped<ITransformData, TransformData>();
                        services.AddScoped<INorthcrestLocalData, NorthcrestLocalData>();
                        services.AddScoped<IRetrievePlanDataService, RetrievePlanDataService>();
                        services.AddScoped<IRetrieveSermonDataService, RetrieveSermonDataService>();
                        services.AddScoped<IRetrieveGeneralSongDataService, RetrieveGeneralSongDataService>();
                        services.AddScoped<IRetrievePlanSongDataService, RetrievePlanSongDataService>();
                        services.AddScoped<IRetrieveItemsService, RetrieveItemsService>();
                        services.AddScoped<INorthcrestConfigurationService, NorthcrestConfigurationService>();
                        services.AddScoped<IDatabaseUpdateService, DatabaseUpdateService>();
                    })
                    .UseSerilog()
                    .Build();

                    var svc = ActivatorUtilities.CreateInstance<RetrievePlanningCenterDataService>(host.Services);
                    await svc.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "There was an error during the Northcrest data refresh.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
