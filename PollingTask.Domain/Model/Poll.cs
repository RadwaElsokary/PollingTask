using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollingTask.Domain.Model
{
    public class Poll
    {
        public int Id { get; set; }
        public string Title { get; set; }
        
        public ICollection<Question> Questions { get; set; }
    }
}
