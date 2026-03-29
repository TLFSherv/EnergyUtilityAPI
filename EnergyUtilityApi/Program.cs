using Microsoft.EntityFrameworkCore;
using EnergyUtilityApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("DbConnection");

builder.Services.AddDbContext<EnergyUtilityDbContext>( // register DbContext
    options => options.UseNpgsql(connString) // specify provider
);
// create a problem details response for caught errors
builder.Services.AddProblemDetails();
// add controllers
builder.Services.AddControllers();
// register custom service
builder.Services.AddScoped<EnergyUtilityService>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();
// add middleware to use controllers
app.MapControllers();
app.Run();
