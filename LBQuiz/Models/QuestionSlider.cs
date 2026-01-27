namespace LBQuiz.Models
{
    public class QuestionSlider : Question
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int? CorrectValue { get; set; }
    }
}
