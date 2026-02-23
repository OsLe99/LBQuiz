using LBQuiz.Data;
using LBQuiz.Models;
using LBQuiz.Models.Helpers;
using LBQuiz.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using LBQuiz.Models.Helpers.AnswerDTO;
using Newtonsoft.Json.Linq;
using LBQuiz.Migrations;

namespace LBQuiz.Services
{
    public class QuestionManager : IQuestionManager
    {
        private readonly ApplicationDbContext _dbContext;

        public QuestionManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;  
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
                SortOrder = question.SortOrder,
                Blob = json,
                QuestionType = "Open"

            };
            _dbContext.QuestionJsonBlobs.Add(blob);
            await _dbContext.SaveChangesAsync();

            
        }
        public async Task CreateSliderQuestion(int quizId, int minValue, int maxValue, int correctValue, int points, string questionText)
        {
            var sO = await GetSortOrderAsync(quizId);
            var question = new QuestionSlider
            {
                QuizId = quizId,
                MinValue = minValue,
                MaxValue = maxValue,
                CorrectValue = correctValue,
                QuestionText = questionText,
                Points = points,
                SortOrder = sO
            };
            var json = JsonSerializer.Serialize(question);
            var blob = new QuestionJsonBlob
            {
                QuizId = quizId,
                QuestionText = questionText,
                SortOrder = question.SortOrder,
                Blob = json,
                QuestionType = "Slider"
            };
            _dbContext.QuestionJsonBlobs.Add(blob);
            await _dbContext.SaveChangesAsync();

        }
        public async Task CreateMultipleChoiceQuestion(int quizId, int questionPoints, string questionText, List<MultipleOptions> multiple)
        {
            var sO = await GetSortOrderAsync(quizId);

            var dto = new MultipleChoiceQuestionDTO
            {
                Points = questionPoints,
                MultipleOptionsList = multiple
            };

            var jsonBlob = new QuestionJsonBlob
            {
                QuizId = quizId,
                QuestionText = questionText,
                SortOrder = sO,
                Blob = JsonSerializer.Serialize(dto),
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

        public async Task<string> GetQuestionTypeStringAsync(QuestionJsonBlob question)
        {
            return question.QuestionType;
        }
        public async Task<List<QuestionJsonBlob>> GetAllQuestionJsonBlobAsync(int quizId)
        {
            return await _dbContext.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToListAsync();
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
            var updateQuestion = await _dbContext.QuestionJsonBlobs.Where(q => q.Id == question.Id).FirstOrDefaultAsync();
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
                var multipleChoice = JsonSerializer.Deserialize<MultipleChoice>(updateQuestion.Blob);
                multipleChoice.Points = points;
                var json = JsonSerializer.Serialize(multipleChoice);
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

        //public async Task UpdateSortOrderAsync(int quizId, int oldIndex, int newIndex)
        //{
        //    //List of all questions wich need sortorder updated
        //    var allQuestion = await _dbContext.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToListAsync();
        //    newIndex = newIndex + 1;
        //    foreach (var question in allQuestion)
        //    {
        //        if (question == null) continue;

        //        if (question.SortOrder == oldIndex)
        //        {
        //            question.SortOrder = newIndex;
        //        }
        //        else if (oldIndex < newIndex)
        //        {
        //            if (question.SortOrder > oldIndex && question.SortOrder <= newIndex)
        //            {
        //                question.SortOrder--;
        //            }
        //        }
        //        else if (oldIndex > newIndex)
        //        {
        //            if (question.SortOrder >= newIndex && question.SortOrder < oldIndex)
        //            {
        //                question.SortOrder++;
        //            }

        //        }
        //    }
        //    await _dbContext.SaveChangesAsync();
        //}
        public async Task UpdateSortOrderAsync(List<QuestionJsonBlob> allQuestions)
        {
            _dbContext.UpdateRange(allQuestions);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateQuestionText(Question question)
        {
            if(question is QuestionOpen quest)
            {
                var questionUpdate = _dbContext.QuestionJsonBlobs.Where(q => q.Id == question.Id).FirstOrDefault();
                questionUpdate.QuestionText = question.QuestionText;
                var openQuest = JsonSerializer.Deserialize<QuestionOpen>(questionUpdate.Blob);
                openQuest.QuestionText = quest.QuestionText;
                openQuest.Points = question.Points;
                openQuest.CorrectAnswer = quest.CorrectAnswer;

                var json = JsonSerializer.Serialize<QuestionOpen>(openQuest);
                questionUpdate.Blob = json;

                _dbContext.Update(questionUpdate);
                await _dbContext.SaveChangesAsync();
            }
            else if(question is QuestionSlider slider)
            {
                var questionUpdate = _dbContext.QuestionJsonBlobs.Where(q => q.Id == slider.Id).FirstOrDefault();
                questionUpdate.QuestionText = question.QuestionText;

                var openQuest = JsonSerializer.Deserialize<QuestionSlider>(questionUpdate.Blob);
                openQuest.QuestionText = slider.QuestionText;
                openQuest.MaxValue = slider.MaxValue;
                openQuest.MinValue = slider.MinValue;
                openQuest.CorrectValue = slider.CorrectValue;
                var json = JsonSerializer.Serialize<QuestionSlider>(openQuest);
                questionUpdate.Blob = json;


                _dbContext.Update(questionUpdate);
                await _dbContext.SaveChangesAsync();



            }
            else if(question is MultipleChoice multiple)
            {
                var questionUpdate = _dbContext.QuestionJsonBlobs.Where(q => q.Id == question.Id).FirstOrDefault();
                questionUpdate.QuestionText = question.QuestionText;

                var dto = new MultipleChoiceQuestionDTO
                {
                    Points = multiple.Points,
                    MultipleOptionsList = multiple.MultipleOptionsList
                };

                questionUpdate.Blob = JsonSerializer.Serialize(dto);

                _dbContext.Update(questionUpdate);
                await _dbContext.SaveChangesAsync();
            }
            
            
        }

        public async Task DeleteQuestionAsync(QuestionJsonBlob question)
        {
            _dbContext.QuestionJsonBlobs.Remove(question);
            await _dbContext.SaveChangesAsync();
        }


        #endregion

        public async Task<Question> GetQuestionFromBlob(QuestionJsonBlob questionJsonBlob)
        {
            if (questionJsonBlob.QuestionType == "Open")
            {
                var question = JsonSerializer.Deserialize<QuestionOpen>(questionJsonBlob.Blob);
                question.Id = questionJsonBlob.Id;
                return question;
            }
            if (questionJsonBlob.QuestionType == "Slider")
            {
                var question = JsonSerializer.Deserialize<QuestionSlider>(questionJsonBlob.Blob);
                question.Id = questionJsonBlob.Id;
                return question;
            }
            if (questionJsonBlob.QuestionType == "Multiple")
            {
                var dto = JsonSerializer.Deserialize<MultipleChoiceQuestionDTO>(questionJsonBlob.Blob);

                var result = new MultipleChoice
                {
                    Id = questionJsonBlob.Id,
                    QuizId = questionJsonBlob.QuizId,
                    QuestionText = questionJsonBlob.QuestionText,
                    SortOrder = questionJsonBlob.SortOrder,
                    Points = dto.Points,
                    MultipleOptionsList = dto.MultipleOptionsList
                };

                return result;
            }
            
            return null;
        }
    }
}
