using Microsoft.EntityFrameworkCore;
using EnergyUtilityApi;
using FluentValidation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("DbConnection");

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// specify the allowed origins for CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
        {
            policy.WithOrigins("http://localhost:5094*")
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
