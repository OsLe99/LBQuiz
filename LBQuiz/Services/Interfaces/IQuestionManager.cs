namespace LBQuiz.Services.Interfaces
{
    public interface IQuestionManager
    {
        List<Models.QuestionOpen> GetAllQuestionFromQuizId(int questionId);
    }
}
