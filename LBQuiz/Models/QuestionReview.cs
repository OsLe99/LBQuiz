using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models;

public class QuestionReview : Question
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinValue >= MaxValue)
        {
            yield return new ValidationResult("Min value must be less than Max value", new[] { nameof(MinValue), nameof(MaxValue) });
        }
    }
}