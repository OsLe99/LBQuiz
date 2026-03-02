using LBQuiz.Data;
using LBQuiz.Migrations;
using LBQuiz.Models;
using LBQuiz.Models.Helpers;
using LBQuiz.Models.Helpers.AnswerDTO;
using LBQuiz.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;

namespace LBQuiz.Services
{
    public class QuestionManager : IQuestionManager
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public QuestionManager(IDbContextFactory<ApplicationDbContext> dbContext)
        {
            _factory = dbContext;  
        }
        
        #region CRUD Operations - Create
        public async Task CreateOpenQuestion(int quizId, string questionText, string correctAnswer, int points)
        {
            using var context = await _factory.CreateDbContextAsync();
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
            context.QuestionJsonBlobs.Add(blob);
            await context.SaveChangesAsync();

            
        }
        public async Task CreateSliderQuestion(int quizId, int minValue, int maxValue, int correctValue, int points, string questionText)
        {
            using var context = await _factory.CreateDbContextAsync();
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
            context.QuestionJsonBlobs.Add(blob);
            await context.SaveChangesAsync();

        }
        public async Task CreateMultipleChoiceQuestion(int quizId, int questionPoints, string questionText, List<MultipleOptions> multiple)
        {
            using var context = await _factory.CreateDbContextAsync();
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
                context.QuestionJsonBlobs.Add(jsonBlob);
                await context.SaveChangesAsync();
            }
        }

        public async Task CreateWordCloudQuestion(int quizId, string questionText, int maxEntries)
        {
            using var context = await _factory.CreateDbContextAsync();
            var sO = await GetSortOrderAsync(quizId);

            var dto = new WordCloudQuestionDTO
            {
                Points = 0,
                MaxEntries = maxEntries
            };

            var jsonBlob = new QuestionJsonBlob
            {
                QuizId = quizId,
                QuestionText = questionText,
                SortOrder = sO,
                Blob = JsonSerializer.Serialize(dto),
                QuestionType = "WordCloud"
            };
            context.QuestionJsonBlobs.Add(jsonBlob);
            await context.SaveChangesAsync();
        }

        public async Task CreateReviewQuestion(int quizId, string questionText, int minValue, int maxValue, int points)
        {
            using var context = await _factory.CreateDbContextAsync();
            var sO = await GetSortOrderAsync(quizId);

            var dto = new ReviewQuestionDTO
            {
                Points = points,
                MinValue = minValue,
                MaxValue = maxValue
            };

            var jsonBlob = new QuestionJsonBlob
            {
                QuizId = quizId,
                QuestionText = questionText,
                SortOrder = sO,
                Blob = JsonSerializer.Serialize(dto),
                QuestionType = "Review"
            };
            context.QuestionJsonBlobs.Add(jsonBlob);
            await context.SaveChangesAsync();
        }
        
        #endregion
        
        #region CRUD Operations - Update & Delete
        public async Task<QuestionJsonBlob> UpdateQuestionTextAsync(Question question, string questionText)
        {
            using var context = await _factory.CreateDbContextAsync();
            var updateQuestion = await context.QuestionJsonBlobs.Where(q => q.Id == question.Id).FirstOrDefaultAsync();
            if(updateQuestion != null)
            {
                updateQuestion.QuestionText = questionText;
            }
            context.QuestionJsonBlobs.Update(updateQuestion);
            await context.SaveChangesAsync();
            return updateQuestion;
        }
        public async Task<QuestionJsonBlob> UpdateQuestionPointsAsync(Question question, int points)
        {
            using var context = await _factory.CreateDbContextAsync();
            var updateQuestion = await context.QuestionJsonBlobs.Where(q => q.Id == question.QuizId).FirstOrDefaultAsync();
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

        public async Task UpdateSortOrderAsync(List<QuestionJsonBlob> allQuestions)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.UpdateRange(allQuestions);
            await context.SaveChangesAsync();
        }
        public async Task UpdateQuestionText(Question question)
        {
            using var context = await _factory.CreateDbContextAsync();
            if (question is QuestionOpen quest)
            {
                var questionUpdate = context.QuestionJsonBlobs.Where(q => q.Id == question.Id).FirstOrDefault();
                questionUpdate.QuestionText = question.QuestionText;
                var openQuest = JsonSerializer.Deserialize<QuestionOpen>(questionUpdate.Blob);
                openQuest.QuestionText = quest.QuestionText;
                openQuest.Points = question.Points;
                openQuest.CorrectAnswer = quest.CorrectAnswer;

                var json = JsonSerializer.Serialize<QuestionOpen>(openQuest);
                questionUpdate.Blob = json;

                context.Update(questionUpdate);
                await context.SaveChangesAsync();
            }
            else if(question is QuestionSlider slider)
            {
                var questionUpdate = context.QuestionJsonBlobs.Where(q => q.Id == slider.Id).FirstOrDefault();
                questionUpdate.QuestionText = question.QuestionText;

                var openQuest = JsonSerializer.Deserialize<QuestionSlider>(questionUpdate.Blob);
                openQuest.QuestionText = slider.QuestionText;
                openQuest.MaxValue = slider.MaxValue;
                openQuest.MinValue = slider.MinValue;
                openQuest.CorrectValue = slider.CorrectValue;
                var json = JsonSerializer.Serialize<QuestionSlider>(openQuest);
                questionUpdate.Blob = json;


                context.Update(questionUpdate);
                await context.SaveChangesAsync();



            }
            else if(question is MultipleChoice multiple)
            {
                var questionUpdate = context.QuestionJsonBlobs.Where(q => q.Id == question.Id).FirstOrDefault();
                questionUpdate.QuestionText = question.QuestionText;

                var dto = new MultipleChoiceQuestionDTO
                {
                    Points = multiple.Points,
                    MultipleOptionsList = multiple.MultipleOptionsList
                };

                questionUpdate.Blob = JsonSerializer.Serialize(dto);

                context.Update(questionUpdate);
                await context.SaveChangesAsync();
            }
            else if (question is QuestionWordCloud wordCloud)
            {
                var questionUpdate = context.QuestionJsonBlobs.Where(q => q.Id == question.Id).FirstOrDefault();
                questionUpdate.QuestionText = wordCloud.QuestionText;

                var dto = new WordCloudQuestionDTO
                {
                    Points = wordCloud.Points,
                    MaxEntries = wordCloud.MaxEntries
                };
                
                questionUpdate.Blob = JsonSerializer.Serialize(dto);

                context.Update(questionUpdate);
                await context.SaveChangesAsync();
            }
        }
        
        public async Task DeleteQuestionAsync(Question question)
        {
            using var context = await _factory.CreateDbContextAsync();
            var deleteQuestion = await context.QuestionJsonBlobs.Where(q => q.Id == question.Id).FirstOrDefaultAsync();
            context.Remove(deleteQuestion);
            await context.SaveChangesAsync();
        }
        
        public async Task DeleteQuestionAsync(QuestionJsonBlob question)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.QuestionJsonBlobs.Remove(question);
            await context.SaveChangesAsync();
        }


        #endregion

        public async Task<int> GetSortOrderAsync(int quizId)
        {
            using var context = await _factory.CreateDbContextAsync();
            int sOrder = context.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToList().Count + 1;
            return sOrder;

        }

        public async Task<string> GetQuestionTypeStringAsync(QuestionJsonBlob question)
        {
            return question.QuestionType;
        }
        public async Task<List<QuestionJsonBlob>> GetAllQuestionJsonBlobAsync(int quizId)
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToListAsync();
        }
        
        public async Task<QuestionJsonBlob> GetQuestionJsonBlobFromQuestionIdAsync(int questionId)
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.QuestionJsonBlobs.Where(q => q.Id == questionId).SingleOrDefaultAsync();
        }
        public async Task<int> GetNumberOfQuestionInQuizAsync(int quizId)
        {
            using var context = await _factory.CreateDbContextAsync();
            return context.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToList().Count;
        }
        
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

            if (questionJsonBlob.QuestionType == "WordCloud")
            {
                var dto = JsonSerializer.Deserialize<WordCloudQuestionDTO>(questionJsonBlob.Blob);

                var result = new QuestionWordCloud
                {
                    Id = questionJsonBlob.Id,
                    QuizId = questionJsonBlob.QuizId,
                    QuestionText = questionJsonBlob.QuestionText,
                    SortOrder = questionJsonBlob.SortOrder,
                    Points = dto.Points,
                    MaxEntries = dto.MaxEntries
                };
                return result;
            }
            
            if (questionJsonBlob.QuestionType == "Review")
            {
                var dto = JsonSerializer.Deserialize<ReviewQuestionDTO>(questionJsonBlob.Blob);

                var result = new QuestionReview
                {
                    Id = questionJsonBlob.Id,
                    QuizId = questionJsonBlob.QuizId,
                    QuestionText = questionJsonBlob.QuestionText,
                    SortOrder = questionJsonBlob.SortOrder,
                    Points = dto.Points,
                    MinValue = dto.MinValue,
                    MaxValue = dto.MaxValue
                };
                return result;
            }
            
            return null;
        }
        
        public async Task<bool> ReturnBoolOnAnswer(QuestionJsonBlob question, string answer)
        {
            bool questionBool = false;
            if (question.QuestionType == "Open")
            {
                var quest = JsonSerializer.Deserialize<QuestionOpen>(question.Blob);
                if (quest.CorrectAnswer.ToLower() == answer.ToLower())
                {
                    questionBool = true;
                }
            }
            else if(question.QuestionType == "Slider")
            {
                var quest = JsonSerializer.Deserialize<QuestionSlider>(question.Blob);
                if(quest.CorrectValue.ToString() == answer)
                {
                    questionBool = true;
                }
            }
            else if(question.QuestionType == "Review")
            {
                // Review questions don't have correct answers, any valid numeric answer is accepted
                questionBool = int.TryParse(answer, out _);
            }
            return questionBool;
        }
        public async Task<int> ReturnMaxIndexForQuestion(int quizId)
        {
            using var context = await _factory.CreateDbContextAsync();
            return context.QuestionJsonBlobs.Where(q => q.QuizId == quizId).ToList().Count;
        }
    }
}
