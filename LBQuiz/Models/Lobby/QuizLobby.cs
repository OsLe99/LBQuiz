using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Models.Lobby;

public class QuizLobby
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "QuizId is required")]
    public int QuizId { get; set; }

    [Required(ErrorMessage = "JoinCode is required")]
    [StringLength(6, MinimumLength = 4, ErrorMessage = "QuizCode must be between 4 and 6 characters")]
    public string JoinCode { get; set; }
    
    [Required(ErrorMessage = "Quiz creation date is required")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "Quiz bool statement is required")]
    public bool IsActive { get; set; } = true;

    public QuizLobby()
    {
        JoinCode = GenerateJoinCode();
    }

    private string GenerateJoinCode(int length = 6)
    {
        const string validChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(validChars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static bool IsValidJoinCode(string code)
    {
        return !string.IsNullOrEmpty(code) && 
               code.Length >= 4 &&
               code.Length <= 6 &&
               code.All(c => "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".Contains(char.ToUpper(c)));
    }
}