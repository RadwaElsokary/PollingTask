using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollingTask.Domain.Model
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Poll Poll { get; set; }
        public int PollId { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}
