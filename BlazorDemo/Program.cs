using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorDemo.Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//Conf Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("Foo service", () =>
        HealthCheckResult.Degraded("foo Service Check Did Not Work Well!"), new[] {"service"})
    .AddCheck("Bar service", () =>
        HealthCheckResult.Healthy("Bar Service Check Worked!"),new[] {"service"})
    .AddCheck("database", () =>
        HealthCheckResult.Healthy("Database Check Worked!"), new[] {"database", "SQL"});
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/quick_health", new HealthCheckOptions()
    {
        Predicate = _ => false
    });
    endpoints.MapHealthChecks("/health/services", new HealthCheckOptions()
    {
        Predicate = reg => reg.Tags.Contains("service"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

    });
    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();