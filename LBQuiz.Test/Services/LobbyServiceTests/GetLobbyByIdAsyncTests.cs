using LBQuiz.Data;
using LBQuiz.Services;
using LBQuiz.Models.Lobby;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Test.Services.LobbyServiceTests;

public class GetLobbyByIdAsyncTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetLobbyByIdAsync_WithValidId_ShouldReturnLobby()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby = new QuizLobby
        {
            Id = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act
        var result = await service.GetLobbyByIdAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }
    
    [Fact]
    public async Task GetLobbyByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby = new QuizLobby
        {
            Id = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act
        var result = await service.GetLobbyByIdAsync(2);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLobbyByIdAsync_MultipleLobbies_ShouldReturnCorrectLobby()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby1 = new QuizLobby
        {
            Id = 1,
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        var lobby2 = new QuizLobby
        {
            Id = 2,
            QuizId = 2,
            JoinCode = "DEF123",
            QuizHostId = "host456",
            IsActive = true
        };
        context.AddRange(lobby1, lobby2);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act
        var result = await service.GetLobbyByIdAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.QuizId);
        Assert.Equal("ABC123", result.JoinCode);
        Assert.Equal("host123", result.QuizHostId);
    }

    [Fact]
    public async Task GetLobbyById_InactiveLobby_ShouldReturnLobby()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby = new QuizLobby
        {
            Id = 1,
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = false
        };
        context.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(context);
        
        // Act
        var result = await service.GetLobbyByIdAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.QuizId);
        Assert.Equal("ABC123", result.JoinCode);
        Assert.Equal("host123", result.QuizHostId);
        Assert.False(result.IsActive);
    }
}