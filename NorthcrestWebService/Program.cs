using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using NorthcrestWebService.App_BusinessLogic.ManifestUnit;
using NorthcrestWebService.App_BusinessLogic.Song;
using NorthcrestWebService.Common.Factories;
using NorthcrestWebService.Common.FactoryInterfaces;
using NorthcrestWebService.Models;
using NorthcrestWebService.Models.Interfaces;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options => {
        // send back a ISO date
        var settings = options.SerializerSettings;
        settings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
    });

builder.Services.AddScoped<ISermonProcessor, SermonProcessor>()
    .AddScoped<IClientAttachmentFactory, ClientAttachmentFactory>()
    .AddScoped<IClientSermonFactory, ClientSermonFactory>()
    .AddScoped<ISongProcessor, SongProcessor>()
    .AddScoped<IClientNoteFactory, ClientNoteFactory>()
    .AddScoped<IClientPlan_ForSongsFactory, ClientPlan_ForSongsFactory>()
    .AddScoped<IClientSongFactory, ClientSongFactory>()
    .AddScoped<IClientAttachment, ClientAttachment>()
    .AddScoped<IClientGeneralSong, ClientGeneralSong>()
    .AddScoped<IClientPlan_ForSongs, ClientPlan_ForSongs>()
    .AddScoped<IClientPlanSong, ClientPlanSong>()
    .AddScoped<IClientSermon, ClientSermon>()
    .AddScoped<IClientSongAttachment, ClientSongAttachment>()
    .AddScoped<IClientSongNote, ClientSongNote>();

//Added the below code to access the site during development.  Otherwise, the web app can't access
// the site due to CORS policy.  I can take out later.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          //policy.WithOrigins("https://app.northcrestbaptist.com/:443");
                          policy.AllowAnyHeader();
                          policy.AllowAnyOrigin();
                          policy.AllowAnyMethod();
                      });
});

JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     //.AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAd");
     .AddMicrosoftIdentityWebApi(options =>
     {
         builder.Configuration.Bind("AzureAd", options);
         options.Events = new JwtBearerEvents();

         /// <summary>
         /// Below you can do extended token validation and check for additional claims, such as:
         ///
         /// - check if the caller's tenant is in the allowed tenants list via the 'tid' claim (for multi-tenant applications)
         /// - check if the caller's account is homed or guest via the 'acct' optional claim
         /// - check if the caller belongs to right roles or groups via the 'roles' or 'groups' claim, respectively
         ///
         /// Bear in mind that you can do any of the above checks within the individual routes and/or controllers as well.
         /// For more information, visit: https://docs.microsoft.com/azure/active-directory/develop/access-tokens#validate-the-user-has-permission-to-access-this-data
         /// </summary>

         //options.Events.OnTokenValidated = async context =>
         //{
         //    string[] allowedClientApps = { /* list of client ids to allow */ };

         //    string clientappId = context?.Principal?.Claims
         //        .FirstOrDefault(x => x.Type == "azp" || x.Type == "appid")?.Value;

         //    if (!allowedClientApps.Contains(clientappId))
         //    {
         //        throw new System.Exception("This client is not authorized");
         //    }
         //};
     }, options => { builder.Configuration.Bind("AzureAd", options); });
//builder.Services.AddAuthorization(authorizationOptions =>
//{
//    authorizationOptions.AddPolicy(
//        "UserCanViewSermonNotes", AuthorizationPolicies.CanAccessSermonNotes());
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
