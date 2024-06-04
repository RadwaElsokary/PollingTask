namespace PollingTask.Presentation.ViewModels
{
    public class LastQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string? PollText { get; set; }
        public string? QuestionText { get; set; }
        public string? AnswerText { get; set; }
        public List<AnswerViewModel>? Answers { get; set; }
        public int SelectedAnswerId { get; set; }
    }
}
