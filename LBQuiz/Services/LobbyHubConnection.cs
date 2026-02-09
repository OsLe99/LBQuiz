using LBQuiz.Models;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Concurrent;

namespace LBQuiz.Services
{
    public class LobbyHubConnection : ILobbyHubConnection
    {
        private HubConnection? _hubConnection;
        private int? _currentLobbyId;
        private string? _currentUserId;
        public List<LobbyParticipant> Participants { get; private set; } = new();

        public event Func<Task>? OnParticipantsChanged;

        public event Func<int, Task>? OnQuestionChanged;

        public event Func<bool, List<LobbyParticipant>, Task>? OnResultShow;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public int? CurrentLobbyId => _currentLobbyId;

        public event Func<string, Models.Lobby.LobbyParticipant, Task>? OnAnswerRecieved;
        public event Func<string, Models.QuestionOpen, Models.Lobby.LobbyParticipant, Task>? OnCalculateScoreBoard;


        
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

            _hubConnection.On<string, int, string>("QuizLobbyStarted",
                (joinCode, lobbyId, hostId) =>
                {
                    // If host, go to host page
                    if (_currentUserId != null && _currentUserId == hostId)
                    {
                        navigation.NavigateTo($"/quiz/host/{joinCode}");
                    }
                    // All other users
                    else
                    {
                        navigation.NavigateTo($"/quiz/play/{joinCode}");
                    }
                });

            // Listen for server event with corrected name and payload
            _hubConnection.On<string, int, Models.Lobby.LobbyParticipant>("AnswerReceived",
                async (answer, questionId, participant) =>
                {
                    if(OnAnswerRecieved != null)
                    {
                        await OnAnswerRecieved.Invoke(answer, participant);
                    }
                    
                });

            _hubConnection.On<Models.QuestionOpen, string, Models.Lobby.LobbyParticipant>("ScoreBoardCalculated",
                async (question, answer, participant) =>
                {
                    // Score is now calculated server-side, just pass it through
                    Console.WriteLine($"{participant.Nickname}: {participant.Score} points");
                    if (OnCalculateScoreBoard != null)
                    {
                        await OnCalculateScoreBoard.Invoke(answer, question, participant);
                    }
                });

            _hubConnection.On<int>("GoToNextQuestion", async (questionIndex) =>
            {
                questionIndex++;
                if (OnQuestionChanged != null)
                {
                    await OnQuestionChanged.Invoke(questionIndex);
                }
                
            });
            _hubConnection.On<int>("GoToPreviousQuestion", async (questionIndex) =>
            {
                questionIndex--;
                if (OnQuestionChanged != null)
                {
                    await OnQuestionChanged.Invoke(questionIndex);
                }
            });

            _hubConnection.On<bool, List<LobbyParticipant>>("GoToResults", async (showResults, scoreBoard) =>
            {
                if (OnResultShow != null)
                {
                    await OnResultShow.Invoke(showResults, scoreBoard);
                }
            });
            
            _hubConnection.On("QuizEnded", async () =>
            {
                navigation.NavigateTo("/");
                await _hubConnection.StopAsync();
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
        public async Task StartQuizAsync(string joinCode, int lobbyId, string hostId)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync("StartQuiz", joinCode, lobbyId, hostId);
            }
        }
        
        public async Task SubmitAnswer(string lobbyId, string answer, int quizId)
        {
            if(_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("ReceiveSubmittedAnswer", answer, lobbyId, quizId);
            }
        }
        public async Task UpdateScoreBoard(Models.QuestionOpen Question, string answer)
        {
            if(_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("CalculateScoreBoard", Question, answer);
            }
        }

        public async Task GoToNextQuestionAsync(int questionIndex, string lobbyId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("GoToNextQuestionAsync", questionIndex, lobbyId);
            }
        }

        public async Task GoToPreviousQuestionAsync(int questionIndex, string lobbyId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("GoToPreviousQuestionAsync", questionIndex, lobbyId);
            }
        }

        public async Task GoToResultsAsync(bool showResults, string lobbyId, List<LobbyParticipant> lobbyScore)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("GoToResultsAsync", showResults, lobbyId, lobbyScore);
            }
        }

        public async Task EndQuizAsync(string lobbyId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("EndQuiz", lobbyId);
            }
        }
        

        
    }
}
