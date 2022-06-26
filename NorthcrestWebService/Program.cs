using NorthcrestWebService.App_BusinessLogic.Interfaces;
using NorthcrestWebService.App_BusinessLogic.ManifestUnit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ISermonProcessor, SermonProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
