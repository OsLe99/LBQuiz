using LBQuiz.Models;
using LBQuiz.Models.Helpers;
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
        public event Func<ChatMessage, Task>? OnMessageRecived;

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

            _hubConnection.On<ChatMessage>("ReceiveMessage", async (playMessage) =>
            {
                if(OnMessageRecived != null)
                {
                    await OnMessageRecived.Invoke(playMessage);
                }
            });

            await _hubConnection.StartAsync();
        }

        public async Task JoinLobbyChatAsync(string lobbyId)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("JoinLobbyChat", lobbyId);
            }
        }

        public async Task SendMessage(ChatMessage playMessage)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("SendMessages", playMessage);
            }
        }
    }
}
