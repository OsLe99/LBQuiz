using LBQuiz.Data;
using LBQuiz.Models.Lobby;
using LBQuiz.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LBQuiz.Test.Services.LobbyServiceTests;

public class GetLobbyByJoinCodeAsyncTests
{
    private IDbContextFactory<ApplicationDbContext> CreateInMemoryFactory()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var factory = new PooledDbContextFactory<ApplicationDbContext>(options);

        return factory;
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_WithValidActiveCode_ShouldReturnLobby()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
        var lobby = new QuizLobby
        {
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.QuizLobby.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(factory);
        
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
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
        var lobby = new QuizLobby
        {
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.QuizLobby.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(factory);
        
        // Act
        var result = await service.GetLobbyByJoinCodeAsync("123ABC");
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_WithValidCodeInactiveLobby_ShouldReturnNull()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
        var lobby = new QuizLobby
        {
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = false
        };
        context.QuizLobby.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(factory);
        
        // Act
        var result = await service.GetLobbyByJoinCodeAsync("ABC123");
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_WithEmptyDatabase_ShouldReturnNull()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
        var service = new LobbyService(factory);
        
        // Act
        var result = await service.GetLobbyByJoinCodeAsync("ABC123");
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_NotCaseSensitive_ShouldReturnLobby()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
        var lobby = new QuizLobby
        {
            QuizId = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.QuizLobby.Add(lobby);
        await context.SaveChangesAsync();
        var service = new LobbyService(factory);
        
        // Act
        var result = await service.GetLobbyByJoinCodeAsync("abc123");
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetLobbyByJoinCodeAsync_MultipleLobbies_ShouldReturnCorrectLobby()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
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
        
        var service = new LobbyService(factory);
        
        //Act
        var result = await service.GetLobbyByJoinCodeAsync("DEF123");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("DEF123", result.JoinCode);
        Assert.Equal(2, result.QuizId);
    }
}