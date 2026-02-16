using LBQuiz.Models;
using LBQuiz.Models.Helpers;
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
        Task StartQuizAsync(int lobbyId, int quizId);
        List<LobbyParticipant> Participants { get; }
        event Func<Task>? OnParticipantsChanged;
        event Func<int, Task>? OnQuestionChanged;
        event Func<string, LobbyParticipant, Task>? OnAnswerRecieved;
        event Func<bool, List<LobbyParticipant>, Task>? OnResultShow;
        event Func<string, Models.QuestionOpen, LobbyParticipant, Task>? OnCalculateScoreBoard;
        event Func<int, int, LobbyParticipant, string, Task>? OnShowSliderValueToHost;
        event Func<LobbyParticipant, int, List<MultipleOptions>, int, Task>? OnShowMultipleAnswersToHost;
        Task SubmitAnswer(int lobbyId, string answer, int quizId);
        Task UpdateScoreBoard(Models.Question question, string answer);
        Task GoToNextQuestionAsync(int questionIndex, int lobbyId);
        Task GoToPreviousQuestionAsync(int questionIndex, int lobbyId);
        Task GoToResultsAsync(bool showResults, int lobbyId, List<LobbyParticipant> lobbyScore);
        Task EndQuizAsync(int lobbyId);
        Task SubmitSliderAnswer(int lobbyId, int sliderValue, int quizId, string questionText);
        Task SubmitMulitpleAnswers(int lobbyId, int quizId, List<MultipleOptions> participantAnswers, int questionId);


    }
}
