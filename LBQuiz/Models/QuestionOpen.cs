using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models
{
    public class QuestionOpen : Question
    {
        [Required(ErrorMessage = "Correct answer is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Answer must be between 1 and 100 characters")]
        public string CorrectAnswer { get; set; }
    }
}
