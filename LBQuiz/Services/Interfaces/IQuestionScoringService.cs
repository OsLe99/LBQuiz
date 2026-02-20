using LBQuiz.Models;

namespace LBQuiz.Services.Interfaces;

public interface IQuestionScoringService
{
    bool IsCorrect(QuestionJsonBlob question, string answer, out int points);
}