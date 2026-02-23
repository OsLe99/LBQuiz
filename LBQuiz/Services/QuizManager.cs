using LBQuiz.Data;
using LBQuiz.Models;
using LBQuiz.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Services
{
    public class QuizManager : IQuizManager
    {
        private readonly ApplicationDbContext _dbContext;
        public QuizManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Quiz>> GetAllQuizesFromHostAsync(string hostId) 
        {
            return await _dbContext.Quiz.Where(q => q.HostId == hostId).ToListAsync();
        }

        public async Task<string> GetHostIdFromQuiz(int quizId)
        {
            var result = _dbContext.Quiz.Where(q => q.Id == quizId).FirstOrDefault();
            return result.HostId;
        }
        public async Task DeleteQuizAsync(Quiz quiz)
        {
            var questions = _dbContext.QuestionJsonBlobs.Where(q => q.QuizId == quiz.Id).ToList();

            if(questions != null)
            {
                _dbContext.RemoveRange(questions);
            }
            _dbContext.Quiz.Remove(quiz);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateQuizAsync(Quiz quiz)
        {
            _dbContext.Quiz.Update(quiz);
            await _dbContext.SaveChangesAsync();
        }
    }
}
