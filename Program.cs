
using cse325_Team6_Project.Components;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using MyMuscleCars.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Net;
using Serilog;

// Load environment variables
Env.Load();


// Configure Serilog Logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/app-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Replace default logging with Serilog
builder.Host.UseSerilog();

// Database setup
var connectionString = Env.GetString("DATABASE_URL")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Blazor setup
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Global Exception Handling (500)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        if (exceptionFeature?.Error is Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred while processing {Path}", context.Request.Path);
        }

        // For API endpoints, return JSON
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected server error occurred.",
                requestId = context.TraceIdentifier
            });
        }
        else
        {
            context.Response.Redirect("/Error/500");
        }
    });
});

// Status Code Handling (401, 404, 500)
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

    var statusCode = response.StatusCode;
    var path = context.HttpContext.Request.Path;

    logger.LogWarning("Status code {StatusCode} encountered for path {Path}", statusCode, path);

    if (path.StartsWithSegments("/api"))
    {
        // Return JSON for API calls
        response.ContentType = "application/json";
        await response.WriteAsJsonAsync(new
        {
            error = statusCode switch
            {
                401 => "Unauthorized access.",
                404 => "The requested resource was not found.",
                _ => "An unexpected error occurred."
            },
            status = statusCode,
            requestId = context.HttpContext.TraceIdentifier
        });
    }
    else
    {
        // Redirect to friendly Razor error pages
        switch (statusCode)
        {
            case 401:
                response.Redirect("/Error/401");
                break;
            case 404:
                response.Redirect("/Error/404");
                break;
            default:
                response.Redirect("/Error/500");
                break;
        }
    }
});

// Security, static files, and routing
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

