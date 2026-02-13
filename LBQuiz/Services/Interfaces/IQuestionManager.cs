using LBQuiz.Models;
using LBQuiz.Models.Helpers;

namespace LBQuiz.Services.Interfaces
{
    public interface IQuestionManager
    {
        List<Models.QuestionOpen> GetAllQuestionFromQuizId(int questionId);
        Task CreateOpenQuestion(int quizId, string questionText, string correctAnswer, int points);
        Task CreateSliderQuestion(int quizId, int minValue, int maxValue, int? correctValue, string questionText);
        Task CreateMultipleChoiceQuestion(int quizId, int questionPoints, string questionText, List<MultipleOptions> multiple);
        Task<QuestionJsonBlob> GetQuestionJsonBlobAsync(int quizId);
        Task<int> GetSortOrderAsync(int quizId);
        Task<string> GetQuestionTypeStringAsync(QuestionJsonBlob question);
        Task<List<QuestionJsonBlob>> GetAllQuestionJsonBlobAsync(int quizId);
        Task<QuestionMultiple> GetQuestionMultipleFromQuestionIdAsync(int questionId);
    }
}
