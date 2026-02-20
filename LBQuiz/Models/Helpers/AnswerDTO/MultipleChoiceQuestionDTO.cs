using System.Text.Json.Serialization;

namespace LBQuiz.Models.Helpers.AnswerDTO;

public class MultipleChoiceQuestionDTO
{
    public List<MultipleOptions> MultipleOptionsList { get; set; } = new();
    public int Points { get; set; }
}