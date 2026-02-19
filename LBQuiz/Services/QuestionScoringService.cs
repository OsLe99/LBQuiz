using LBQuiz.Models;
using LBQuiz.Models.Helpers;
using LBQuiz.Models.Helpers.AnswerDTO;
using LBQuiz.Services.Interfaces;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LBQuiz.Services;

public class QuestionScoringService : IQuestionScoringService
{
    public bool IsCorrect(QuestionJsonBlob question, string answer, out int points)
    {
        points = 0;

        if (question == null)
            throw new ArgumentNullException(nameof(question));

        if (string.IsNullOrWhiteSpace(answer))
            return false;

        return question.QuestionType switch
        {
            "Open" => ScoreOpen(question, answer, out points),
            "Slider" => ScoreSlider(question, answer, out points),
            "Multiple" => ScoreMultiple(question, answer, out points),
            _ => false
        };
    }
    
    private bool ScoreOpen(QuestionJsonBlob question, string answer, out int points)
    {
        points = 0;
        var open = JsonSerializer.Deserialize<OpenQuestionDTO>(question.Blob);
        if (open != null && open.CorrectAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase))
        {
            points = open.Points;
            return true;
        }

        return false;
    }

    private bool ScoreSlider(QuestionJsonBlob question, string answer, out int points)
    {
        points = 0;
        var slider = JsonSerializer.Deserialize<SliderQuestionDTO>(question.Blob);
        if (slider != null && slider.CorrectValue.Equals(int.Parse(answer)))
        {
            points = slider.Points;
            return true;
        }

        return false;
    }

    private bool ScoreMultiple(QuestionJsonBlob question, string answer, out int points)
    {
        points = 0;
        
        var multiple = JsonSerializer.Deserialize<MultipleChoiceQuestionDTO>(question.Blob);
        if (multiple == null || !multiple.MultipleOptionsList.Any())
            return false;
        
        var correctIds = multiple.MultipleOptionsList
            .Where(o => o.CorrectFalse)
            .Select(o => o.Id)
            .ToList();
        
        var submittedIds = JsonSerializer.Deserialize<List<int>>(answer);
        if (submittedIds == null || !submittedIds.Any())
            return false;
        
        int pointsPerCorrect = multiple.Points / correctIds.Count;
        
        int correctSelections = submittedIds.Count(id => correctIds.Contains(id));
        int wrongSelections = submittedIds.Count(id => !correctIds.Contains(id));
        
        int calculatedPoints = (correctSelections * pointsPerCorrect) 
                               - (wrongSelections * pointsPerCorrect);
        points = Math.Max(0, calculatedPoints);

        return points > 0;
    }
    
}