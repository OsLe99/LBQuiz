using LBQuiz.Models;

namespace LBQuiz.Services.Interfaces
{
    public interface IQuestionManager
    {
        List<Models.QuestionOpen> GetAllQuestionFromQuizId(int questionId);
        Task CreateOpenQuestion(int quizId, string questionText, string correctAnswer, int points);
        Task CreateSliderQuestion(int quizId, int minValue, int maxValue, int? correctValue, string questionText);
        Task CreateMultipleChoiceQuestion(List<MultipleChoiceAnswer> multiple);
    }
}
