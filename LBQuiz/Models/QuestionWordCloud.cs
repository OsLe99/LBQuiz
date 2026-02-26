using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models;

public class QuestionWordCloud : Question
{
    [Range(1, 10, ErrorMessage = "A range of entries must be between 1 and 10")]
    public required int MaxEntries { get; set; } = 3;
}