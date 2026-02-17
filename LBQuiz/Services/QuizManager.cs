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


    }
}
