namespace LBQuiz.Models
{
    public class QuestionMultiple : Question
    {
        public List<String> CorrectAnswers { get; set; }
        public List<String> AllAnswers { get; set; }
    }
}
