using LBQuiz.Data;
using LBQuiz.Models.Lobby;
using LBQuiz.Services;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Test.Services.LobbyServiceTests;

public class EndQuizAsyncTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task EndQuizAsync_WithExistingLobby_ShouldSetIsActiveToFalse()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby = new QuizLobby
        {
            Id = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true,
        };
        context.Add(lobby);
        await context.SaveChangesAsync();
        var lobbyService = new LobbyService(context);
        
        // Act
        var result = lobbyService.EndQuizAsync(1);
        await lobbyService.EndQuizAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        var updatedLobby = await context.QuizLobby.FindAsync(1);
        Assert.NotNull(updatedLobby);
        Assert.False(updatedLobby.IsActive);
    }

    [Fact]
    public async Task EndQuizAsync_ShouldNotAffectOtherLobbies()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby1 = new QuizLobby
        {
            Id = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true,
        };
        var lobby2 = new QuizLobby
        {
            Id = 2,
            JoinCode = "DEF123",
            QuizHostId = "host456",
            IsActive = true
        };
        
        context.AddRange(lobby1, lobby2);
        await context.SaveChangesAsync();
        
        var lobbyService = new LobbyService(context);
        
        // Act
        var result = lobbyService.EndQuizAsync(1);
        await lobbyService.EndQuizAsync(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        var updatedLobby = await context.QuizLobby.FindAsync(1);
        Assert.NotNull(updatedLobby);
        Assert.False(updatedLobby.IsActive);
    }

    [Fact]
    public async Task EndQuizAsync_WithAlreadyInactiveLobby_ShouldRemainInactive()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobby = new QuizLobby
        {
            Id = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = false
        };
        context.Add(lobby);
        await context.SaveChangesAsync();
        
        var lobbyService = new LobbyService(context);
        
        // Act
        await lobbyService.EndQuizAsync(1);
        
        // Assert
        var updatedLobby = await context.QuizLobby.FindAsync(1);
        Assert.False(updatedLobby?.IsActive);
    }
    
    [Fact]
    public async Task EndQuizAsync_NonExistentLobby_ShouldNotThrow()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var lobbyService = new LobbyService(context);
        
        // Act & Assert
        await lobbyService.EndQuizAsync(1);
    }
}