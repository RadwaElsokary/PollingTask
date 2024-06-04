using PollingTask.Domain.Model;

namespace PollingTask.Presentation.ViewModels
{
    public class ViewPollViewModel
    {
        public int  PollId { get; set; }
        public string PollText { get; set; }
        public List<Question> Questions { get; set; }
    }
}
