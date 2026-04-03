using HomeAutomation.Application;
using HomeAutomation.CloudInverter;
using HomeAutomation.LocalInverter;
using HomeAutomation.MetOffice;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddOptions<LocalInverterApiOptions>()
    .Bind(builder.Configuration.GetSection("Services:LocalInverterApiSettingsOptions"))
    .ValidateDataAnnotations();

builder.Services
    .AddOptions<CloudInverterApiOptions>()
    .Bind(builder.Configuration.GetSection("Services:CloudInverterApiSettingsOptions"))
    .ValidateDataAnnotations();

builder.Services
    .AddOptions<OpenMeteoApiOptions>()
    .Bind(builder.Configuration.GetSection("Services:OpenMeteoApiSettingsOptions"))
    .ValidateDataAnnotations();

builder.Services
    .AddOptions<HomeAutomation.Application.BatteryData.BatteryOptions>()
    .Bind(builder.Configuration.GetSection("Services:BatteryOptions"));

builder.Services.AddControllersWithViews();

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks();

builder.Services.RegisterApplicationServices();
builder.Services.RegisterCloudInverterServices();
builder.Services.RegisterLocalInverterServices();
builder.Services.RegisterWeatherServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/error");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();


app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();