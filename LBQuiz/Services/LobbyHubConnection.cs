using LBQuiz.Models;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using LBQuiz.Models.Helpers;
using System.Runtime.InteropServices;

namespace LBQuiz.Services
{
    public class LobbyHubConnection : ILobbyHubConnection
    {
        private HubConnection? _hubConnection;
        private int? _currentLobbyId;
        private string? _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public List<LobbyParticipant> Participants { get; private set; } = new();

        public event Func<Task>? OnParticipantsChanged;

        public event Func<int, Task>? OnQuestionChanged;

        public event Func<bool, List<LobbyParticipant>, Task>? OnResultShow;
        public event Func<int, int, LobbyParticipant, string, Task>? OnShowSliderValueToHost;
        public event Func<LobbyParticipant, int, List<MultipleOptions>, int, Task>? OnShowMultipleAnswersToHost;
        public event Func<string, QuestionJsonBlob, Task>? OnPointsDeducted;
        public event Func<string, QuestionJsonBlob, Task>? OnPointsAwarded;
        public event Func<int, Task>? OnCountDownStart;


        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public int? CurrentLobbyId => _currentLobbyId;
        public string? ConnectionId => _hubConnection?.ConnectionId;

        public event Func<string, LobbyParticipant, Task>? OnAnswerRecieved;
        public event Func<string, QuestionJsonBlob, LobbyParticipant, int, Task>? OnCalculateScoreBoard;

        public LobbyHubConnection(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor =  httpContextAccessor;
        }
        
        public async Task InitializeAsync(NavigationManager navigation, string? userId = null)
        {
            if (_hubConnection?.State == HubConnectionState.Connected) return;
            
            _currentUserId = userId;
            
            var httpContext = _httpContextAccessor.HttpContext;
            var cookies = httpContext?.Request.Cookies;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigation.ToAbsoluteUri("/lobbyHub"), options =>
                {
                    if (cookies != null)
                    {
                        options.Headers.Add("Cookie", string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}")));
                    }
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

            _hubConnection.On<string, string, int>("QuizLobbyStarted",
                async (joinCode, hostId, countDownTimer) =>
                {
                    if(OnCountDownStart != null)
                    {
                        await OnCountDownStart.Invoke(countDownTimer);
                    }
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
            _hubConnection.On<string, int, LobbyParticipant>("AnswerReceived",
                async (answer, questionId, participant) =>
                {
                    if(OnAnswerRecieved != null)
                    {
                        await OnAnswerRecieved.Invoke(answer, participant);
                    }
                });

            _hubConnection.On<QuestionJsonBlob, string, LobbyParticipant, int>("ScoreBoardCalculated",
                async (question, answer, participant, points) =>
                {
                    if (OnCalculateScoreBoard != null)
                    {
                        await OnCalculateScoreBoard.Invoke(answer, question, participant, points);
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

            _hubConnection.On<int, int, LobbyParticipant, string>("SliderAnswerSubmit", async (sliderValue, quizId, participant, questionText) =>
            {
                if (OnShowSliderValueToHost != null)
                {
                    await OnShowSliderValueToHost.Invoke(sliderValue, quizId, participant, questionText);
                }
            });
            _hubConnection.On<LobbyParticipant, int, List<MultipleOptions>, int>("MultipleAnswersSubmits", async (participant, quizId, participantAnswers, questionId) =>
            {
                if(OnShowMultipleAnswersToHost != null)
                {
                    await OnShowMultipleAnswersToHost.Invoke(participant, quizId, participantAnswers, questionId);
                }
            });
            _hubConnection.On<string, QuestionJsonBlob>("OnDeductPoints", async (nickName, question) =>
            {
                if(OnPointsDeducted != null)
                {
                    await OnPointsDeducted.Invoke(nickName, question);
                }
            });
            _hubConnection.On<string, QuestionJsonBlob>("OnAwardPoints", async (nickName, question) =>
            {
                if(OnPointsAwarded != null)
                {
                    await OnPointsAwarded.Invoke(nickName, question);
                }
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

        public async Task RejoinLobbyAsync(int lobbyId, string nickname)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("RejoinLobby", lobbyId, nickname);
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
        public async Task StartQuizAsync(int lobbyId, int quizId, int countDownTimer)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync("StartQuiz", lobbyId, quizId, countDownTimer);
            }
        }
        
        public async Task SubmitAnswer(int lobbyId, string answer, int quizId)
        {
            if(_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("ReceiveSubmittedAnswer", answer, lobbyId.ToString(), quizId);
            }
        }
        public async Task UpdateScoreBoard(int questionId, string answer)
        {
            if(_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("CalculateScoreBoard", questionId, answer);
            }
        }

        public async Task GoToNextQuestionAsync(int questionIndex, int lobbyId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("GoToNextQuestionAsync", questionIndex, lobbyId);
            }
        }

        public async Task GoToPreviousQuestionAsync(int questionIndex, int lobbyId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("GoToPreviousQuestionAsync", questionIndex, lobbyId);
            }
        }

        public async Task GoToResultsAsync(bool showResults, int lobbyId, List<LobbyParticipant> lobbyScore)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("GoToResultsAsync", showResults, lobbyId, lobbyScore);
            }
        }

        public async Task EndQuizAsync(int lobbyId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("EndQuiz", lobbyId);
            }
        }
        public async Task SubmitSliderAnswer(int lobbyId, int sliderValue, int quizId, string questionText)
        {
            if(_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("SubmitSliderAnswer", lobbyId, sliderValue, quizId, questionText);
            }
        }
        public async Task SubmitMultipleAnswers(int lobbyId, int quizId, List<MultipleOptions> participantAnswers, int questionId)
        {
            if(_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("SubmitMultipleAnswers", lobbyId, quizId, participantAnswers, questionId);
            }
        }
        public async Task DeductPoints(string nickName, QuestionJsonBlob question, int lobbyId)
        {
            if(_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("DeductPoints", nickName, question, lobbyId);
            }
        }

        public async Task AwardPoints(string nickName, QuestionJsonBlob question, int lobbyId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.InvokeAsync("AwardPoints", nickName, question, lobbyId);
            }
        }

    }
}
