using LBQuiz.Data;
using LBQuiz.Models;
using LBQuiz.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

        public async Task CreateOpenQuestion(int quizId, string questionText, string correctAnswer, int points)
        {
            var sO = await GetSortOrderAsync(quizId);
            var question = new QuestionOpen()
            {
                QuizId = quizId,
                QuestionText = questionText,
                CorrectAnswer = correctAnswer,
                Points = points,
                SortOrder = sO
            };
            var json = JsonSerializer.Serialize(question);
            var blob = new QuestionJsonBlob
            {
                QuizId = quizId,
                Blob = json
            };
            _dbContext.QuestionJsonBlobs.Add(blob);
            await _dbContext.SaveChangesAsync();

            
        }
        public async Task CreateSliderQuestion(int quizId, int minValue, int maxValue, int? correctValue, string questionText)
        {
            var sO = await GetSortOrderAsync(quizId);
            var question = new QuestionSlider()
            {
                QuizId = quizId,
                MinValue = minValue,
                MaxValue = maxValue,
                CorrectValue = correctValue,
                QuestionText = questionText,
                SortOrder = sO
            };
            var json = JsonSerializer.Serialize(question);
            var blob = new QuestionJsonBlob
            {
                QuizId = quizId,
                Blob = json
            };
            _dbContext.QuestionJsonBlobs.Add(blob);
            await _dbContext.SaveChangesAsync();

        }
        public async Task CreateMultipleChoiceQuestion(List<MultipleChoiceAnswer> multiple)
        {
            //.Add till denna lista 
            var questionMultiple = new QuestionMultiple();
            //Sånna här vi ska skapa 
            foreach (var item in multiple)
            {
                questionMultiple.AllAnswers.Add(item);
            }
        }



        public async Task<int> GetSortOrderAsync(int quizId)
        {
            int sOrder = _dbContext.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToList().Count + 1;
            return sOrder;

        }
    }
}
