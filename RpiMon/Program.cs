using RpiMon.Components;
using RpiMon.Services;
using RpiMon.Hubs;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Configure data protection to persist keys
builder.Services.AddDataProtection()
    .SetApplicationName("RpiMon")
    .PersistKeysToFileSystem(new DirectoryInfo("/tmp/dataprotection-keys"));
// Configure URLs programmatically to avoid environment variable conflicts
// This prevents the "Overriding HTTP_PORTS/HTTPS_PORTS" warning that occurs
// when ASPNETCORE_URLS environment variable conflicts with other port settings
builder.WebHost.UseUrls("http://+:5000");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();

// Register services
builder.Services.AddScoped<ISystemInfoService, SystemInfoService>();
builder.Services.AddHostedService<SystemInfoBackgroundService>();

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
// Antiforgery disabled - not needed for read-only monitoring application
// app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map SignalR hub
app.MapHub<SystemInfoHub>("/systeminfohub");

app.Run();
