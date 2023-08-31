using System.Net.Http.Headers;
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
    .AddOptions<MetOfficeApiOptions>()
    .Bind(builder.Configuration.GetSection("Services:MetOfficeApiSettingsOptions"))
    .ValidateDataAnnotations();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<HttpClient>(c => c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")));

builder.Services.RegisterApplicationServices();
builder.Services.RegisterCloudInverterServices();
builder.Services.RegisterLocalInverterServices();
builder.Services.RegisterWeatherServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();