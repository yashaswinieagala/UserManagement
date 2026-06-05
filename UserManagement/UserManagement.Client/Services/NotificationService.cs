using Microsoft.AspNetCore.SignalR.Client;
using UserManagement.Shared.Models;

namespace UserManagement.Client.Services
{
    public class NotificationService : IAsyncDisposable
    {
        private HubConnection? _hub;
        public bool IsConnected => _hub?.State == HubConnectionState.Connected;
        public event Action<UserNotification>? OnNotification;
        public event Action? OnConnectionChanged;

        public async Task ConnectAsync(string baseUrl)
        {
            if (_hub != null) return;

            var hubUrl = baseUrl.TrimEnd('/') + "/userhub";

            _hub = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hub.On<UserNotification>("ReceiveNotification", n =>
            {
                OnNotification?.Invoke(n);
            });

            _hub.Reconnected += _ => { OnConnectionChanged?.Invoke(); return Task.CompletedTask; };
            _hub.Reconnecting += _ => { OnConnectionChanged?.Invoke(); return Task.CompletedTask; };
            _hub.Closed += _ => { OnConnectionChanged?.Invoke(); return Task.CompletedTask; };

            try { await _hub.StartAsync(); }
            catch { /* silently fail; UI shows offline state */ }
            OnConnectionChanged?.Invoke();
        }

        public async ValueTask DisposeAsync()
        {
            if (_hub != null)
                await _hub.DisposeAsync();
        }
    }
}
