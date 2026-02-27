using LBQuiz.Data;
using LBQuiz.Models.Lobby;
using LBQuiz.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LBQuiz.Test.Services.LobbyServiceTests;

public class GetLobbyByIdAsyncTests
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
    public async Task GetLobbyByIdAsync_WithValidId_ShouldReturnLobby()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
        var lobby = new QuizLobby
        {
            Id = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(factory);
        
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
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
        var lobby = new QuizLobby
        {
            Id = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true
        };
        context.Add(lobby);
        await context.SaveChangesAsync();
        
        var service = new LobbyService(factory);
        
        // Act
        var result = await service.GetLobbyByIdAsync(2);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLobbyByIdAsync_MultipleLobbies_ShouldReturnCorrectLobby()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
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
        
        var service = new LobbyService(factory);
        
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
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
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
        
        var service = new LobbyService(factory);
        
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