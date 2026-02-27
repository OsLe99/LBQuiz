using LBQuiz.Data;
using LBQuiz.Models;
using LBQuiz.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace LBQuiz.Services
{
    public class QuizManager : IQuizManager
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        public QuizManager(IDbContextFactory<ApplicationDbContext> dbContext)
        {
            _factory = dbContext;
        }

        

        public async Task<List<Quiz>> GetAllQuizesFromHostAsync(string hostId) 
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Quiz.Where(q => q.HostId == hostId).ToListAsync();
        }

        public async Task<string> GetHostIdFromQuiz(int quizId)
        {
            using var context = await _factory.CreateDbContextAsync();
            var result = context.Quiz.Where(q => q.Id == quizId).FirstOrDefault();
            return result.HostId;
        }
        public async Task DeleteQuizAsync(Quiz quiz)
        {
            using var context = await _factory.CreateDbContextAsync();
            var questions = context.QuestionJsonBlobs.Where(q => q.QuizId == quiz.Id).ToList();

            if(questions != null)
            {
                context.RemoveRange(questions);
            }
            context.Quiz.Remove(quiz);
            await context.SaveChangesAsync();
        }
        public async Task UpdateQuizAsync(Quiz quiz)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.Quiz.Update(quiz);
            await context.SaveChangesAsync();
            
        }
    }
}
