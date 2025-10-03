
using cse325_Team6_Project.Components;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using MyMuscleCars.Data;
using MyMuscleCars.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Get connection string from environment variable (from .env)
var connectionString = Env.GetString("DATABASE_URL")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers(); //this registers controllers

// Register EF Core DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register ContactService
builder.Services.AddScoped<IContactService, ContactService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();

app.MapControllers(); //this maps controller routes

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
