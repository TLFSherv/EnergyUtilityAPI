using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.getConnectionString("DefaultConnection");

builder.Services.AddDbContext<EnergyUtilityDbContext>( // register DbContext
    options => options.UseNpgsql(connString) // specify provider
);
// create a problem details response for caught errors
builder.Services.AddProblemDetails();
// add controllers
builder.Services.AddControllers();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();
// add middleware to use controllers
app.MapControllers();
app.Run();
