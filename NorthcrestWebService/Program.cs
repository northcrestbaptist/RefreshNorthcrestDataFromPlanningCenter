using NorthcrestWebService.App_BusinessLogic.Interfaces;
using NorthcrestWebService.App_BusinessLogic.ManifestUnit;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
         // The below option will maintain the case of the properties during serialization.
         // I determined that I don't need this on the client.
        //opts.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddScoped<ISermonProcessor, SermonProcessor>();

// Added the below code to access the site during development.  Otherwise, the web app can't access
// the site due to CORS policy.  I can take out later.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://azure01.northcrestbaptist.com/:443");
                      });
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();
app.MapControllers();

app.Run();
