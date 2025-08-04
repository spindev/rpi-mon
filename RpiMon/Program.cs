using RpiMon.Components;
using RpiMon.Services;
using RpiMon.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs to avoid port binding warnings
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
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map SignalR hub
app.MapHub<SystemInfoHub>("/systeminfohub");

app.Run();
