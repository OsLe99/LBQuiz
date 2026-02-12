namespace LBQuiz.Models
{
    public class QuestionJsonBlob
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string Blob {  get; set; }
    }
}
