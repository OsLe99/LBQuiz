using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models
{
    public class MultipleChoiceAnswer
    {
        public int Id { get; set; }
    
        [Required]
        public int QuestionId { get; set; }
    
        [Required(ErrorMessage = "Answer text is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Answer must be between 1 and 100 characters")]
        public string AnswerText { get; set; }
    
        [Required]
        public bool IsCorrectAnswer { get; set; }
    
        public virtual QuestionMultiple Question { get; set; }
    }
}