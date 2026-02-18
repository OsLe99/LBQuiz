using LBQuiz.Models;
using LBQuiz.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace LBQuiz.Services.ChatHub
{
    public class ChatHubConnection : IChatHubConnection
    {
        private HubConnection? _hubConnection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string? _currentUserId;
        public event Func<string, string, Task>? OnMessageRecived;

        public ChatHubConnection(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InitializeAsync(NavigationManager navigation, string? userId = null)
        {
            if (_hubConnection?.State == HubConnectionState.Connected) return;

            _currentUserId = userId;

            var httpContext = _httpContextAccessor.HttpContext;
            var cookies = httpContext?.Request.Cookies;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigation.ToAbsoluteUri("/chathub"), options =>
                {
                    if (cookies != null)
                    {
                        options.Headers.Add("Cookie", string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}")));
                    }
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", async (user, message) =>
            {
                if(OnMessageRecived != null)
                {
                    await OnMessageRecived.Invoke(user, message);
                }
            });

            await _hubConnection.StartAsync();
        }

        public async Task SendMessage(string userName, string message)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("SendMessages", userName, message);
            }
        }
    }
}
