using LBQuiz.Data;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Services
{
    public class QuizManager
    {
        private readonly ApplicationDbContext _dbContext;
        public QuizManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateQuiz(string questionText, int points)
        {
            //Hämta listan med quesiton och sätt sortOrder till count + 1
        }


    }
}
