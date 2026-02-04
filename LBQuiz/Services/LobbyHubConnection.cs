using LBQuiz.Models;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LBQuiz.Services
{
    public class LobbyHubConnection : ILobbyHubConnection
    {
        private HubConnection? _hubConnection;
        private int? _currentLobbyId;
        private string? _currentUserId;
        public List<LobbyParticipant> Participants { get; private set; } = new();

        public event Func<Task>? OnParticipantsChanged;

        public event Func<Task>? OnQuestionChanged;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public int? CurrentLobbyId => _currentLobbyId;

        public async Task InitializeAsync(NavigationManager navigation, string? userId = null)
        {
            if (_hubConnection?.State == HubConnectionState.Connected) return;
            
            _currentUserId = userId;



            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigation.ToAbsoluteUri("/lobbyHub"), options =>
                {
                    options.UseDefaultCredentials = true;
                })
                .WithAutomaticReconnect()
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

            _hubConnection.On<int, int, string>("QuizLobbyStarted",
                (quizId, lobbyId, hostId) =>
                {
                    // If host, go to host page
                    if (_currentUserId != null && _currentUserId == hostId)
                    {
                        navigation.NavigateTo($"/quiz/host/{quizId}/{lobbyId}");
                    }
                    // All other users
                    else
                    {
                        navigation.NavigateTo($"/quiz/play/{quizId}/{lobbyId}");
                    }
                });

            _hubConnection.On<int, int, string>("ShowQuestion", async (quizId, lobbyId, questionText) =>
            {
                await OnQuestionChanged.Invoke();
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

        public async Task JoinLobbyAsHostAsync(int lobbyId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("JoinLobbyAsHost", lobbyId);
                _currentLobbyId = lobbyId;
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
        public async Task StartQuizAsync(int lobbyId, int quizId, string hostId)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync("StartQuiz", lobbyId, quizId, hostId);
            }
        }
        
        public async Task ShowQuestionAsync(int questionId, int lobbyId, string questionText)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync("ShowQuestion", questionId, questionText);
            }
        }
    }
}
