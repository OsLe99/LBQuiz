using LBQuiz.Data;
using LBQuiz.Models;
using LBQuiz.Models.Helpers;
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
                QuestionText = questionText,
                Blob = json,
                QuestionType = "Open"

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
                QuestionText = questionText,
                Blob = json,
                QuestionType = "Slider"
            };
            _dbContext.QuestionJsonBlobs.Add(blob);
            await _dbContext.SaveChangesAsync();

        }
        public async Task CreateMultipleChoiceQuestion(int quizId, int questionPoints, string questionText, List<MultipleOptions> multiple)
        {
            //Måste skapa en QuestionMultiple först innan vi kan spara, behöver quizId till MultipleChoicAnswer
            var sO = await GetSortOrderAsync(quizId);
            var multipleQuestion = new Models.MultipleChoiceAnswer() { 
                QuizId = quizId,
                QuestionText = questionText,
                Points = questionPoints,
                SortOrder= sO,
                MultipleOptionsList = multiple
            };
            var jsonBlob = new QuestionJsonBlob()
            {
                QuizId = quizId,
                QuestionText = questionText,
                Blob = JsonSerializer.Serialize(multiple),
                QuestionType = "Multiple"
                
            };
            if(jsonBlob != null)
            {
                _dbContext.QuestionJsonBlobs.Add(jsonBlob);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<int> GetSortOrderAsync(int quizId)
        {
            int sOrder = _dbContext.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToList().Count + 1;
            return sOrder;

        }
        public async Task<QuestionJsonBlob> GetQuestionJsonBlobAsync(int quizId)
        {
            return _dbContext.QuestionJsonBlobs.Where(q => q.QuizId == quizId).SingleOrDefault();
        } 

        public async Task<string> GetQuestionTypeStringAsync(QuestionJsonBlob question)
        {
            return question.QuestionType;
        }
        public async Task<List<QuestionJsonBlob>> GetAllQuestionJsonBlobAsync(int quizId)
        {
            return await _dbContext.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToListAsync();
        }

        public async Task<QuestionMultiple> GetQuestionMultipleFromQuestionIdAsync(int questionId)
        {
            return await _dbContext.QuestionMultiple.Where(q => q.Id == questionId).FirstOrDefaultAsync();
        }
        public async Task<QuestionJsonBlob> GetQuestionJsonBlobFromQuestionIdAsync(int questionId)
        {
            return await _dbContext.QuestionJsonBlobs.Where(q => q.Id == questionId).SingleOrDefaultAsync();
        }
        public async Task<int> GetNumberOfQuestionInQuizAsync(int quizId)
        {
            return _dbContext.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToList().Count;
        }

        //QuestionCrud
        #region CRUD Operations
        public async Task<QuestionJsonBlob> UpdateQuestionTextAsync(Question question, string questionText)
        {
            var updateQuestion = await _dbContext.QuestionJsonBlobs.Where(q => q.Id == question.QuizId).FirstOrDefaultAsync();
            if(updateQuestion != null)
            {
                updateQuestion.QuestionText = questionText;
            }
            _dbContext.QuestionJsonBlobs.Update(updateQuestion);
            await _dbContext.SaveChangesAsync();
            return updateQuestion;
        }
        public async Task<QuestionJsonBlob> UpdateQuestionPointsAsync(Question question, int points)
        {
            var updateQuestion = await _dbContext.QuestionJsonBlobs.Where(q => q.Id == question.QuizId).FirstOrDefaultAsync();
            if(updateQuestion.QuestionType == "Open")
            {
                var openQuestion = JsonSerializer.Deserialize<QuestionOpen>(updateQuestion.Blob);
                openQuestion.Points = points;
                var json = JsonSerializer.Serialize(openQuestion);
                updateQuestion.Blob = json;
            }
            if(updateQuestion.QuestionType == "Slider")
            {
                var sliderQuestion = JsonSerializer.Deserialize<QuestionSlider>(updateQuestion.Blob);
                sliderQuestion.Points = points;
                var json = JsonSerializer.Serialize(sliderQuestion);
                updateQuestion.Blob = json;

            }
            if(updateQuestion.QuestionType == "Multiple")
            {
                var multipleQuestion = JsonSerializer.Deserialize<QuestionMultiple>(updateQuestion.Blob);
                multipleQuestion.Points = points;
                var json = JsonSerializer.Serialize(multipleQuestion);
                updateQuestion.Blob = json;
            }
            return updateQuestion;
        }

        public async Task DeleteQuestionAsync(Question question)
        {
            var deleteQuestion = await _dbContext.QuestionJsonBlobs.Where(q => q.Id == question.Id).FirstOrDefaultAsync();
            _dbContext.Remove(deleteQuestion);
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
