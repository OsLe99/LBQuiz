using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models
{
    public class QuestionSlider : Question, IValidatableObject
    {
        [Required(ErrorMessage = "Minimum value is required")]
        public int MinValue { get; set; }
        
        [Required(ErrorMessage = "Maximum value is required")]
        public int MaxValue { get; set; }
        
        public int? CorrectValue { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinValue >= MaxValue)
            {
                yield return new ValidationResult(
                    "MinValue must be less than MaxValue", 
                    new[] { nameof(MinValue), nameof(MaxValue) }
                );
            }

            if (CorrectValue.HasValue && (CorrectValue < MinValue || CorrectValue > MaxValue))
            {
                yield return new ValidationResult(
                    "CorrectValue must be between MinValue and MaxValue",
                    new[] { nameof(CorrectValue) }
                );
            }
        }
    }
}
