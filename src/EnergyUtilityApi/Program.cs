using Microsoft.EntityFrameworkCore;
using EnergyUtilityApi;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("DefaultConnection");

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// specify the allowed origins for CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
        {
            policy.WithOrigins("https://energyutilityapp.onrender.com*")
             .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});

builder.Services.AddDbContext<EnergyUtilityDbContext>( // register DbContext
    options => options.UseNpgsql(connString) // specify provider
);
// create a problem details response for caught errors
builder.Services.AddProblemDetails();
// add controllers
builder.Services.AddControllers();
// register custom service
builder.Services.AddScoped<EnergyUtilityService>();
// register validator
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.Configure<EnergyUtilityServiceSettings>(
    builder.Configuration.GetSection("EnergyUtilityServiceSettings")
);
// register authentication scheme
builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, ApiKeyHandler>("ApiKeyScheme", null);

builder.Services.AddMemoryCache();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
// add CORS middleware
app.UseCors(MyAllowSpecificOrigins);
app.UseStatusCodePages();
// add middleware to use controllers
app.MapControllers();
app.Run();

public partial class Program { }
