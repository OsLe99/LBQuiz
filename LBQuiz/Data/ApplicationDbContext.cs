using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LBQuiz.Models;
using LBQuiz.Models.Lobby;

namespace LBQuiz.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole, string>(options)
{

    public DbSet<Quiz> Quiz { get; set; }
    
    // Question DbSets go here
    public DbSet<Question> Question { get; set; }
    public DbSet<QuestionOpen> QuestionOpen { get; set; }
    public DbSet<QuestionSlider> QuestionSlider { get; set; }
    public DbSet<QuestionMultiple> QuestionMultiple { get; set; }
    
    // Lobby DbSets go here
    public DbSet<QuizLobby> QuizLobby { get; set; }
    public DbSet<QuestionJsonBlob> QuestionJsonBlobs { get; set; }
}
