using LBQuiz.Models.Helpers;
using System.ComponentModel.DataAnnotations;

namespace LBQuiz.Models
{
    public class MultipleChoice : Question
    {
        public List<MultipleOptions> MultipleOptionsList { get; set; }
    }
}