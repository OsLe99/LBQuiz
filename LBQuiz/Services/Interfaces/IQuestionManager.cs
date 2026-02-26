using LBQuiz.Models;
using LBQuiz.Models.Helpers;

namespace LBQuiz.Services.Interfaces
{
    public interface IQuestionManager
    {
        Task CreateOpenQuestion(int quizId, string questionText, string correctAnswer, int points);
        Task CreateSliderQuestion(int quizId, int minValue, int maxValue, int correctValue, int points, string questionText);
        Task CreateMultipleChoiceQuestion(int quizId, int questionPoints, string questionText, List<MultipleOptions> multiple);
        Task CreateWordCloudQuestion(int quizId, string questionText, int maxEntries);
        Task<int> GetSortOrderAsync(int quizId);
        Task<string> GetQuestionTypeStringAsync(QuestionJsonBlob question);
        Task<List<QuestionJsonBlob>> GetAllQuestionJsonBlobAsync(int quizId);
        Task<QuestionJsonBlob> GetQuestionJsonBlobFromQuestionIdAsync(int questionId);
        Task<int> GetNumberOfQuestionInQuizAsync(int quizId);
        Task<QuestionJsonBlob> UpdateQuestionTextAsync(Question question, string questionText);
        Task<QuestionJsonBlob> UpdateQuestionPointsAsync(Question question, int points);
        Task DeleteQuestionAsync(Question question);
        Task UpdateSortOrderAsync(List<QuestionJsonBlob> allQuestions);
        Task<Question> GetQuestionFromBlob(QuestionJsonBlob blob);
        Task UpdateQuestionText(Question question);
        Task DeleteQuestionAsync(QuestionJsonBlob question);
        Task<bool> ReturnBoolOnAnswer(QuestionJsonBlob question, string answer);

    }
}
