using LBQuiz.Models.Helpers;
using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models
{
    public class MultipleChoiceAnswer : Question
    {
        public List<MultipleOptions> MultipleOptionsList { get; set; }
    }
}