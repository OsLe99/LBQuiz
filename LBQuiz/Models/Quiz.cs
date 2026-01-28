using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Quiz Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Quiz name must be between 1 and 100 characters")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Quiz Description is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Quiz description must be between 1 and 500 characters")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Host id is required")]
        public int HostId { get; set; }

        [Required(ErrorMessage = "A quiz must have questions")]
        public List<Question> Questions { get; set; } = new();
    }
}
