using LBQuiz.Services;
using LBQuiz.Data;
using LBQuiz.Models.Lobby;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Test.Services.LobbyServiceTests;

public class GetLobbyByJoinCodeAsyncTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
    
    [Fact]
    public async Task GetLobbyByJoinCodeAsync_WithValidActiveCode_ShouldReturnLobby()
    {
        // Arrange
        await using var context =  CreateInMemoryContext();
        var lobby = new QuizLobby
        {
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.QuizLobby.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act
        var result = await service.GetLobbyByJoinCodeAsync("ABC123");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("ABC123", result.JoinCode);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_WithInvalidCode_ShouldReturnNull()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby = new QuizLobby
        {
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.QuizLobby.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act
        var result = await service.GetLobbyByJoinCodeAsync("123ABC");
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_WithValidCodeInactiveLobby_ShouldReturnNull()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby = new QuizLobby
        {
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = false
        };
        context.QuizLobby.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act
        var result = await service.GetLobbyByJoinCodeAsync("ABC123");
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_WithEmptyDatabase_ShouldReturnNull()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var service = new LobbyService(context);
        
        // Act
        var result = await service.GetLobbyByJoinCodeAsync("ABC123");
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_NotCaseSensitive_ShouldReturnLobby()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby = new QuizLobby
        {
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.QuizLobby.Add(lobby);
        await context.SaveChangesAsync();
        var service = new LobbyService(context);
        
        // Act
        var result = await service.GetLobbyByJoinCodeAsync("abc123");
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_MultipleLobbies_ShouldReturnCorrectLobby()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby1 = new QuizLobby
        {
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        var lobby2 = new QuizLobby
        {
            QuizId = 2,
            JoinCode = "DEF123",
            QuizHostId = "host456",
            IsActive = true
        };
        context.QuizLobby.AddRange(lobby1, lobby2);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        //Act
        var result = await service.GetLobbyByJoinCodeAsync("DEF123");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("DEF123", result.JoinCode);
        Assert.Equal(2, result.QuizId);
    }
}