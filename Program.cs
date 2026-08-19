using CrmDb.Components;
using CrmDb.Models;
using Microsoft.EntityFrameworkCore;
using CrmDb.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
// Program.cs
builder.Services.AddScoped<WebPageScraper>();
builder.Services.AddScoped<CompanyIngestionService>();
builder.Services.AddScoped<IncentiveProcessorService>();
builder.Services.AddScoped<WebSiteScraper>();

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

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
