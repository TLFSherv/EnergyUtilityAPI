using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);
// create a problem details response for caught errors
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();

app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.Run();
