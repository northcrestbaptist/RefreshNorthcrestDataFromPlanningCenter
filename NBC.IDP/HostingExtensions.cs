using Serilog;

namespace NBC.IDP
{
    internal static class HostingExtensions
    {
        private static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            
            // uncomment if you want to add a UI
            builder.Services.AddRazorPages();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("https://azure01.northcrestbaptist.com/:443",
                                          "capacitor://localhost",
                                          "ionic://localhost",
                                          "ionic://com.tools.northcrestbaptistchurch",
                                          "http://localhost",
                                          "http://localhost:8080",
                                          "http://localhost:8100");
                                      policy.AllowAnyHeader();
                                      //policy.AllowAnyOrigin();
                                      policy.AllowAnyMethod();
                                  });
            });

            builder.Services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    
                    // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add a UI
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseIdentityServer();

            // uncomment if you want to add a UI
            app.UseAuthorization();
            app.MapRazorPages().RequireAuthorization();

            return app;
        }
    }
}