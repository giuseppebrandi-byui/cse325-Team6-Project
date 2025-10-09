using cse325_Team6_Project.Components;
using Microsoft.EntityFrameworkCore;
using MyMuscleCars.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// When running on Render (or another container host) the platform provides
// the port to bind via the PORT environment variable. Also listen on all
// interfaces so the container receives traffic from Render's proxy.
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Support forwarded headers (X-Forwarded-For, X-Forwarded-Proto) so
// authentication/URL generation sees the original scheme when behind a proxy.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// First we  Read connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");

// then Register controllers
builder.Services.AddControllers();

builder.Services.AddHttpClient("ServerAPI", client =>
{
    client.BaseAddress = new Uri("Database_Host"); // your backend API base URL
});



// We need to Register EF Core DbContext with Postgres + retry policy
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        )
    )
);
// ✅ Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
    // Also allow the JWT to be read from an HttpOnly cookie named 'jwtToken'
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("jwtToken"))
            {
                context.Token = context.Request.Cookies["jwtToken"];
            }
            return Task.CompletedTask;
        }
    };
});

// Adding Razor components (interactive server components)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Provide cascading authentication state to Blazor components
builder.Services.AddCascadingAuthenticationState();

// Register client-side auth helper
// AuthService removed in favor of built-in AuthenticationStateProvider

var app = builder.Build();

// Ensuring database migrations are applied
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate(); // applies migrations at startup
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying migrations.");
    }
}

// Environment-specific config
if (!app.Environment.IsDevelopment())
{
    // Production: Render terminates TLS at the edge, so do not force HTTPS
    // or enable HSTS here. Instead we enable forwarded headers so the app
    // can correctly observe the original request scheme.
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseForwardedHeaders();
}
else
{
    // Development
    app.UseDeveloperExceptionPage();
    // Do NOT force HTTPS locally
}

// Middlewares
app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
