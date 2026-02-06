using LBQuiz.Models.Lobby;
using Microsoft.AspNetCore.Components;

namespace LBQuiz.Services.Interfaces
{
    public interface ILobbyHubConnection
    {
        Task InitializeAsync(NavigationManager navigation, string? userId = null);
        Task JoinLobbyAsync(string joinCode, string nickname);
        Task JoinLobbyAsHostAsync(int lobbyId);
        Task LeaveLobbyAsync();
        Task StartQuizAsync(int lobbyId, int quizId, string hostId);
        List<LobbyParticipant> Participants { get; }
        event Func<Task>? OnParticipantsChanged;
        event Func<int, Task>? OnQuestionChanged;
        event Func<string, LobbyParticipant, Task>? OnAnswerRecieved;
        event Func<bool, List<LobbyParticipant>, Task>? OnResultShow;
        event Func<string, Models.QuestionOpen, LobbyParticipant, Task>? OnCalculateScoreBoard;
        Task SubmitAnswer(string lobbyId, string answer, int quizId);
        Task UpdateScoreBoard(Models.QuestionOpen question, string answer);
        Task GoToNextQuestionAsync(int questionIndex, string lobbyId);
        Task GoToPreviousQuestionAsync(int questionIndex, string lobbyId);
        Task GoToResultsAsync(bool showResults, string lobbyId, List<LobbyParticipant> lobbyScore);
        Task EndQuizAsync(string lobbyId);
    }
}
