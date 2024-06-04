using PollingTask.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollingTask.Presentation.Models
{
    public interface IPollRepository
    {
        Poll GetPoll(int Id);
        IEnumerable<Poll> GetAllPolls();
        Poll Add(Poll poll);
        Poll Update(Poll pollChanged);
        Poll Delete(int id);
    }
}
