using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole, string>(options)
{

    public DbSet<LBQuiz.Models.Quiz> Quiz { get; set; }
    public DbSet<LBQuiz.Models.Question> Question { get; set; }
    public DbSet<LBQuiz.Models.QuestionOpen> QuestionOpen { get; set; }
    public DbSet<LBQuiz.Models.QuestionSlider> QuestionSlider { get; set; }
    public DbSet<LBQuiz.Models.QuestionMultiple> QuestionMultiple { get; set; }
}
