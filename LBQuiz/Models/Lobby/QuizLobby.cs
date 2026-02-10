using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Models.Lobby;

public class QuizLobby
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "QuizId is required")]
    public int QuizId { get; set; }
    
    [Required(ErrorMessage = "HostId is required")]
    public string QuizHostId { get; set; }

    [Required(ErrorMessage = "JoinCode is required")]
    [StringLength(6, MinimumLength = 4, ErrorMessage = "QuizCode must be between 4 and 6 characters")]
    public string JoinCode { get; set; }
    
    [Required(ErrorMessage = "Quiz creation date is required")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "Quiz bool statement is required")]
    public bool IsActive { get; set; } = true;
}