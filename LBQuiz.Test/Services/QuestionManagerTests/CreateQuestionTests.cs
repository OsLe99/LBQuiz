using LBQuiz.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using LBQuiz.Services;
using LBQuiz.Models.Helpers;
using LBQuiz.Models.Helpers.AnswerDTO;

namespace LBQuiz.Test.Services.QuestionManagerTests
{
    public class CreateQuestionTests
    {
        private ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateQuestionSliderToJsonBlob_WithValidData()
        {
            //Arrange
            await using var context = CreateInMemoryContext();
            var service = new QuestionManager(context);

            //Act
            var result = service.CreateSliderQuestion(1,1,10, 2, 1000,"TestSliderQuestionText");
            var questionJsonBlob = context.QuestionJsonBlobs.Where(x => x.QuizId == 1).FirstOrDefault();
            //Assert
            Assert.NotNull(result);
            Assert.NotNull(questionJsonBlob);
            Assert.Equal(questionJsonBlob.QuestionText, "TestSliderQuestionText");
            Assert.Equal(questionJsonBlob.QuestionType, "Slider");
        }

        [Fact]
        public async Task CreateQuestionOpenToJsonBlob_WithValidData()
        {
            //Arrange
            await using var context = CreateInMemoryContext();
            var service = new QuestionManager(context);

            //Act
            var result = service.CreateOpenQuestion(1, "Test Question", "Test", 1000);
            var questionJsonBlob = context.QuestionJsonBlobs.Where(x => x.QuizId == 1).FirstOrDefault();
            //Assert
            Assert.NotNull(result);
            Assert.NotNull(questionJsonBlob);
            Assert.Equal(questionJsonBlob.QuestionText, "Test Question");
            Assert.Equal(questionJsonBlob.QuestionType, "Open");
        }

        [Fact]
        public async Task CreateQuestionMultipleToJsonBlob_WithValidData()
        {
            //Arrange
            await using var context = CreateInMemoryContext();
            var service = new QuestionManager(context);

            var multiList = new List<MultipleOptions>
            {
                new() { Id = 1, Text = "First", CorrectFalse = true, ColorString = "rgba(0,255,0,0.4)" },
                new() { Id = 2, Text = "Second", CorrectFalse = true, ColorString = "rgba(255,0,0,0.4)" },
                new() { Id = 3, Text = "Third", CorrectFalse = false, ColorString = "rgba(0,255,0,0.4)" },
                new() { Id = 4, Text = "Fourth", CorrectFalse = false, ColorString = "rgba(0,255,0,0.4)" }
            };
            
            //Act
            await service.CreateMultipleChoiceQuestion(1, 1000, "Test Multiple Choice Question", multiList);

            var questionJsonBlob =  await context.QuestionJsonBlobs.FirstOrDefaultAsync(x => x.QuizId == 1);

            var multiple = JsonSerializer.Deserialize<MultipleChoiceQuestionDTO>(questionJsonBlob!.Blob);
            var blobList = multiple!.MultipleOptionsList;

            //Assert
            Assert.NotNull(questionJsonBlob);
            Assert.Equal(1000, multiple.Points);
            Assert.Equal(4, blobList.Count);
            Assert.Equal("First",  blobList[0].Text);
            Assert.Equal("Second", blobList[1].Text);
            Assert.Equal("Third",  blobList[2].Text);
            Assert.Equal("Fourth", blobList[3].Text);
        }

        [Fact]
        public async Task SortorderShouldReturnNextIndex()
        {
            //Arrange
            await using var context = CreateInMemoryContext();
            var service = new QuestionManager(context);

            //Act
            service.CreateOpenQuestion(1, "Test Question1", "Test1", 1000);
            service.CreateOpenQuestion(1, "Test Question2", "Test2", 1000);
            service.CreateOpenQuestion(1, "Test Question3", "Test3", 1000);
            service.CreateOpenQuestion(1, "Test Question4", "Test4", 1000);
            var sortorderIndex = await service.GetSortOrderAsync(1);

            //Assert
            Assert.NotNull(sortorderIndex);
            Assert.Equal(sortorderIndex, 5);

        }
        [Fact]
        public async Task QuestionTypeShouldReturn_Open()
        {
            //Arrange
            await using var context = CreateInMemoryContext();
            var service = new QuestionManager(context);

            //Act
            service.CreateOpenQuestion(1, "Test Question1", "Test1", 1000);
            service.CreateOpenQuestion(1, "Test Question2", "Test2", 1000);
            service.CreateOpenQuestion(1, "Test Question3", "Test3", 1000);
            service.CreateOpenQuestion(1, "Test Question4", "Test4", 1000);
            var question = context.QuestionJsonBlobs.Where(q => q.Id == 1).FirstOrDefault();
            var typeString = await service.GetQuestionTypeStringAsync(question);

            //Assert
            Assert.NotNull(typeString);
            Assert.Equal(typeString, "Open");
        }
    }
}
