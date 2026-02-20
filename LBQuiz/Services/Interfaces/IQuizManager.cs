using LBQuiz.Models;

namespace LBQuiz.Services.Interfaces
{
    public interface IQuizManager
    {
        Task<List<Quiz>> GetAllQuizesFromHostAsync(string hostId);
        Task<string> GetHostIdFromQuiz(int quizId);
    }
}
