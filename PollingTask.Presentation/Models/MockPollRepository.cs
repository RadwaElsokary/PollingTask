using PollingTask.Domain.Model;

namespace PollingTask.Presentation.Models
{
    public class MockPollRepository : IPollRepository
    {
        private List<Poll> polls;

        public MockPollRepository()
        {

        }



        public Poll GetPoll(int Id)
        {
            return polls.FirstOrDefault(e => e.Id == Id);
        }

        Poll IPollRepository.Add(Poll poll)
        {
            poll.Id = polls.Count() > 0 ? polls.Max(e => e.Id) + 1 : 1;
            polls.Add(poll);
            return poll;
        }

        Poll IPollRepository.Delete(int id)
        {
            Poll poll = polls.FirstOrDefault(e => e.Id == id);

            if (poll != null)
            {
                polls.Remove(poll);
            }
            return poll;

        }

        IEnumerable<Poll> IPollRepository.GetAllPolls()
        {
            return polls;
        }

        Poll IPollRepository.Update(Poll PollChanged)
        {
            Poll poll = polls.FirstOrDefault(e => e.Id == PollChanged.Id);
            if (poll != null)
            {
                poll.Title = poll.Title;
                
            }
            return poll;
        }
    }
}
