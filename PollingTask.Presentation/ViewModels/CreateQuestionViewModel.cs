namespace PollingTask.Presentation.ViewModels
{
    public class CreateQuestionViewModel
    {
        public string QuestionText { get; set; }
        public int PollId { get; set; }
        public string? Poll { get; set; }

        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
        public int QuestionId { get; set; } // Add this property

    }
}
