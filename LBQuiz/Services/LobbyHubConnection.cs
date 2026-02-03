using LBQuiz.Models.Lobby;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LBQuiz.Services
{
    public class LobbyHubConnection : ILobbyHubConnection
    {
        private HubConnection? _hubConnection;
        private int? _currentLobbyId;
        public List<LobbyParticipant> Participants { get; private set; } = new();

        public event Func<Task>? OnParticipantsChanged;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public int? CurrentLobbyId => _currentLobbyId;

        public async Task InitializeAsync(NavigationManager navigation)
        {
            if (_hubConnection != null) return;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigation.ToAbsoluteUri("/lobbyHub"))
                .Build();

            _hubConnection.On<string, List<LobbyParticipant>>("ParticipantJoined",
                async (nickname, participants) =>
                {
                    Participants = participants;
                    if (OnParticipantsChanged != null)
                        await OnParticipantsChanged.Invoke(); // vi returnerar Task
                });

            _hubConnection.On<string, List<LobbyParticipant>>("ParticipantLeft",
                async (nickname, participants) =>
                {
                    Participants = participants;
                    if (OnParticipantsChanged != null)
                    await OnParticipantsChanged.Invoke();
                });

            await _hubConnection.StartAsync();
        }

        public async Task JoinLobbyAsync(string joinCode, string nickname)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("JoinLobby", joinCode, nickname);
            }
        }

        public async Task LeaveLobbyAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("LeaveLobby");
                _currentLobbyId = null;
                Participants.Clear();
            }
        }
    }
}
