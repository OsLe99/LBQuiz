using Microsoft.EntityFrameworkCore;

namespace LBQuiz.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuestionString { get; set; }
        public double? TimeLeft { get; set; }
        public int Points { get; set; }
    }
}
