using LBQuiz.Models;

namespace LBQuiz.Services.Interfaces
{
    public interface IQuizManager
    {
        Task<List<Quiz>> GetAllQuizesFromHostAsync(string hostId);
        Task<string> GetHostIdFromQuiz(int quizId);
        Task DeleteQuizAsync(Quiz quiz);
        Task UpdateQuizAsync(Quiz quiz);
        Task<Quiz> GetQuizFromQuizId(int quizId);
    }
}
