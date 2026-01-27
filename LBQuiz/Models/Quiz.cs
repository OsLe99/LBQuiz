namespace LBQuiz.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public int HostId { get; set; }
        public List<Question> Questions { get; set; }
    }
}
