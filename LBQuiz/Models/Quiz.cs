using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Host id is required")]
        public int HostId { get; set; }

        [Required(ErrorMessage = "A quiz must have questions")]
        public List<Question> Questions { get; set; } = new();
    }
}
