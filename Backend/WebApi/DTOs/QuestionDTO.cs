using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.DTOs
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "The question text is required")]
        public string QuestionText { get; set; } = null!;

        public Picture? Picture { get; set; } = null;

        [Required(ErrorMessage = "A question needs answers")]
        public IEnumerable<AnswerDTO> Answers { get; set; }

        [Required(ErrorMessage = "A question needs a position answers")]
        public int QuestionPosition { get; set; }

        [Required(ErrorMessage = "A question needs a specified time")]
        public int QuestionTime { get; set; }

        [Required(ErrorMessage = "A question needs a maximmum amount of points")]
        public int QuestionMaxPoints { get; set; }

    }
}