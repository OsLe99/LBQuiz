using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models
{
    public abstract class Question
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Quiz ID is required")]
        public int QuizId { get; set; }

        [Required(ErrorMessage = "Question text is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Question must be between 1 and 100 characters")]
        public string QuestionText { get; set; }

        [Range(0, 600, ErrorMessage = "Time limit must be between 0 and 600 seconds")]
        public int? TimeLimitSeconds { get; set; }

        [Required(ErrorMessage = "Points are required")]
        [Range(1, 2000, ErrorMessage = "Points must be between 1 and 2000")]
        public int Points { get; set; } = 1000;
    }
}
