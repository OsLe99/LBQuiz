using LBQuiz.Data;
using LBQuiz.Services.Interfaces;

namespace LBQuiz.Services
{
    public class QuestionManager : IQuestionManager
    {
        private readonly ApplicationDbContext _dbContext;

        public QuestionManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;  
        }



        public List<Models.QuestionOpen> GetAllQuestionFromQuizId(int questionId)
        {
            var list = new List<Models.QuestionOpen>();
            try
            {
                list = _dbContext.QuestionOpen.Where(q => q.QuizId == questionId).ToList();
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Failed to retrive data: " + ex);
            }

            return list;
        }




    }
}
