using LBQuiz.Data;
using LBQuiz.Models.Lobby;
using LBQuiz.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LBQuiz.Test.Services.LobbyServiceTests;

public class EndQuizAsyncTests
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
    public async Task EndQuizAsync_WithExistingLobby_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();

        var lobby = new QuizLobby
        {
            Id = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = true,
        };
        context.Add(lobby);
        await context.SaveChangesAsync();
        var lobbyService = new LobbyService(factory);
        
        // Act
        var result = lobbyService.EndQuizAsync(lobby.Id);
        await lobbyService.EndQuizAsync(lobby.Id);

        // Assert
        using var assertContext = await factory.CreateDbContextAsync();
        var updatedLobby = await assertContext.QuizLobby.FindAsync(1);
        Assert.NotNull(updatedLobby);
        Assert.False(updatedLobby.IsActive);
    }

    [Fact]
    public async Task EndQuizAsync_ShouldNotAffectOtherLobbies()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
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
        
        var lobbyService = new LobbyService(factory);
        
        // Act
        await lobbyService.EndQuizAsync(lobby1.Id);

        // Assert
        using var assertContext = await factory.CreateDbContextAsync();
        var updatedLobby = await assertContext.QuizLobby.Where(q => q.Id == lobby1.Id).SingleOrDefaultAsync();
        Assert.NotNull(updatedLobby);
        Assert.False(updatedLobby.IsActive);
    }

    [Fact]
    public async Task EndQuizAsync_WithAlreadyInactiveLobby_ShouldRemainInactive()
    {
        // Arrange
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
        var lobby = new QuizLobby
        {
            Id = 1,
            JoinCode = "ABC123",
            QuizHostId = "host123",
            IsActive = false
        };
        context.Add(lobby);
        await context.SaveChangesAsync();
        
        var lobbyService = new LobbyService(factory);
        
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
        var factory = CreateInMemoryFactory();

        using var context = await factory.CreateDbContextAsync();
        var lobbyService = new LobbyService(factory);
        
        // Act & Assert
        await lobbyService.EndQuizAsync(1);
    }
}