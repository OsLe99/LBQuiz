using LBQuiz.Data;
using LBQuiz.Models;
using LBQuiz.Models.Lobby;
using LBQuiz.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Services;

public class LobbyService : ILobbyService
{
    private readonly ApplicationDbContext _db;

    public LobbyService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<QuizLobby> CreateLobbyAsync(int quizId)
    {
        var quizExists = await _db.Quiz.AnyAsync(q => q.Id == quizId);

        if (!quizExists)
        {
            // Error handling here
        }
        
        var joinCode = await GenerateUniqueJoinCodeAsync();

        var lobby = new QuizLobby
        {
            QuizId = quizId,
            JoinCode = joinCode,
            CreatedAt = DateTime.UtcNow
        };
        
        _db.QuizLobby.Add(lobby);
        await _db.SaveChangesAsync();
        return lobby;
    }

    public async Task<QuizLobby?> GetLobbyByJoinCodeAsync(string joinCode)
    {
        return await _db.QuizLobby.FirstOrDefaultAsync(q => q.JoinCode == joinCode && q.IsActive);
    }
    public async Task<QuizLobby?> GetLobbyFromJoinCodeAsync(string joinCode)
    {
        return await _db.QuizLobby.FirstOrDefaultAsync(ql => ql.JoinCode == joinCode);
    }

    #region JoinCode Creation

    // Generates a code until we hit one that doesn't exist. Can be made faster
    private async Task<string> GenerateUniqueJoinCodeAsync()
    {
        string code;
        do
        {
            code = GenerateJoinCode();
        } 
        while (await _db.QuizLobby.AnyAsync(q => q.JoinCode == code));
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