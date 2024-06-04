using System.ComponentModel.DataAnnotations;

namespace PollingTask.Presentation.ViewModels
{
    public class AnswerViewModel
    {
        public int Id { get; set; } // Add this property

        [Required(ErrorMessage = "The answer is required.")]
        public string AnswerText { get; set; }

        public int QuestionId { get; set; }
    }
}
