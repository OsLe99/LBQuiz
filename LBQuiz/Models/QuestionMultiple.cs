using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models
{
    public class QuestionMultiple : Question
    {
        [Required(ErrorMessage = "Answer options are required")]
        [MinLength(2, ErrorMessage = "Must have at least 2 options")]
        public virtual ICollection<MultipleChoiceAnswer> AllAnswers { get; set; } = new List<MultipleChoiceAnswer>();
    }
}
