using LBQuiz.Data;
using LBQuiz.Models;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Services;

public class LobbyService : ILobbyService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public LobbyService(IDbContextFactory<ApplicationDbContext> dbContext)
    {
        _factory = dbContext;
    }

    

    public async Task<QuizLobby> CreateLobbyAsync(int quizId, string hostId)
    {
        using var context = await _factory.CreateDbContextAsync();
        var quizExists = await context.Quiz.AnyAsync(q => q.Id == quizId);

        if (!quizExists)
        {
            throw new ArgumentException($"Quiz with id {quizId} does not exist", nameof(quizId));
        }
        
        var joinCode = await GenerateUniqueJoinCodeAsync();

        var lobby = new QuizLobby
        {
            QuizId = quizId,
            JoinCode = joinCode,
            CreatedAt = DateTime.UtcNow,
            QuizHostId = hostId
        };

        context.QuizLobby.Add(lobby);
        await context.SaveChangesAsync();
        return lobby;
    }

    public async Task<QuizLobby?> GetLobbyByJoinCodeAsync(string joinCode)
    {
        using var context = await _factory.CreateDbContextAsync();
        var normalizedJoinCode = joinCode.ToUpperInvariant();
        return await context.QuizLobby.FirstOrDefaultAsync(q => q.JoinCode == normalizedJoinCode && q.IsActive);
    }
    
    public async Task<QuizLobby?> GetLobbyByIdAsync(int lobbyId)
    {
        using var context = await _factory.CreateDbContextAsync();
        return await context.QuizLobby.FirstOrDefaultAsync(ql => ql.Id == lobbyId);
    }

    public async Task EndQuizAsync(int lobbyId)
    {
        using var context = await _factory.CreateDbContextAsync();
        var lobby = await context.QuizLobby.FirstOrDefaultAsync(q => q.Id == lobbyId);
        if (lobby != null)
        {
            lobby.IsActive = false;
            await context.SaveChangesAsync();
        }
    }
    
    #region JoinCode Creation

    // Generates a code until we hit one that doesn't exist. Can be made faster
    private async Task<string> GenerateUniqueJoinCodeAsync()
    {
        using var context = await _factory.CreateDbContextAsync();
        string code;
        do
        {
            code = GenerateJoinCode();
        } 
        while (await context.QuizLobby.AnyAsync(q => q.JoinCode == code));
        return code;
    }
    
    private static string GenerateJoinCode(int length = 6)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = Random.Shared;

        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }


    #endregion
}