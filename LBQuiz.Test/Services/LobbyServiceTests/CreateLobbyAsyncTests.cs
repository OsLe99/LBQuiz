using LBQuiz.Data;
using LBQuiz.Models;
using LBQuiz.Services;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Test.Services.LobbyServiceTests;

public class CreateLobbyAsyncTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateLobbyAsync_WithValidQuiz_ShouldCreateLobby()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var quiz = new Quiz
        {
            Id = 1,
            Name = "Test Quiz",
            Description = "Test Quiz Description",
            HostId = "host123"
        };
        context.Quiz.Add(quiz);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act
        var result = await service.CreateLobbyAsync(1, "host123");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.QuizId);
        Assert.Equal("host123", result.QuizHostId);
        Assert.NotNull(result.JoinCode);
        Assert.Equal(6, result.JoinCode.Length);
        Assert.True(result.IsActive);
        Assert.True(result.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateLobbyAsync_ShouldGenerateUniqueJoinCode()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var quiz = new Quiz
        {
            Id = 1,
            Name = "Test Quiz",
            Description = "Test Quiz Description",
            HostId = "host123"
        };
        context.Quiz.Add(quiz);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act
        var lobby1 = await service.CreateLobbyAsync(1, "host123");
        var lobby2 = await service.CreateLobbyAsync(1, "host456");
        var lobby3 = await service.CreateLobbyAsync(1, "host789");
        
        // Assert
        Assert.NotEqual(lobby1.JoinCode, lobby2.JoinCode);
        Assert.NotEqual(lobby1.JoinCode, lobby3.JoinCode);
        Assert.NotEqual(lobby2.JoinCode, lobby3.JoinCode);
    }
    
    [Fact]
    public async Task CreateLobbyAsync_ShouldPersistToDatabase()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var quiz = new Quiz
        {
            Id = 1,
            Name = "Test Quiz",
            Description = "Test Quiz Description",
            HostId = "host123"
        };
        context.Quiz.Add(quiz);
        await context.SaveChangesAsync();
        var service = new LobbyService(context);
        
        // Act
        var createdLobby = await service.CreateLobbyAsync(1, "host123");
        
        // Assert
        var lobbyFromDb = await context.QuizLobby.FirstOrDefaultAsync(l => l.Id == createdLobby.Id);
        Assert.NotNull(lobbyFromDb);
        Assert.Equal(createdLobby.JoinCode, lobbyFromDb.JoinCode);
        Assert.Equal("host123", lobbyFromDb.QuizHostId);
    }
    
    [Fact]
    public async Task CreateLobbyAsync_WithNonExistingQuiz_ShouldThrowException()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var quiz = new Quiz
        {
            Id = 1,
            Name = "Test Quiz",
            Description = "Test Quiz Description",
            HostId = "host123"
        };
        context.Quiz.Add(quiz);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateLobbyAsync(2, "host123"));
    }
}