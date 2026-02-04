namespace LBQuiz.Services.Interfaces
{
    public interface ILobbyHubService
    {
        Task SubmitAnswerAsync(string answer, int questionId);
    }
}
