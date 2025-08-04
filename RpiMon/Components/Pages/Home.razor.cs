using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using RpiMon.Models;

namespace RpiMon.Components.Pages;

public partial class Home : IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private HubConnection? hubConnection;
    private StaticSystemInfo? staticSystemInfo;
    private DynamicSystemInfo? dynamicSystemInfo;
    private bool isConnected = false;
    private bool isDarkMode = false;
    private bool isLoadingStatic = true;

    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/systeminfohub"))
            .Build();

        hubConnection.On<StaticSystemInfo>("ReceiveStaticSystemInfo", (info) =>
        {
            staticSystemInfo = info;
            isLoadingStatic = false;
            InvokeAsync(StateHasChanged);
        });

        hubConnection.On<DynamicSystemInfo>("ReceiveDynamicSystemInfo", (info) =>
        {
            dynamicSystemInfo = info;
            InvokeAsync(StateHasChanged);
        });

        await hubConnection.StartAsync();
        isConnected = hubConnection.State == HubConnectionState.Connected;
    }

    private string GetTemperatureClass(double temperature)
    {
        return temperature switch
        {
            >= 80 => "text-danger",
            >= 70 => "text-warning",
            _ => "text-success"
        };
    }

    private async Task ToggleTheme()
    {
        isDarkMode = !isDarkMode;
        await JSRuntime.InvokeVoidAsync("toggleTheme", isDarkMode);
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}