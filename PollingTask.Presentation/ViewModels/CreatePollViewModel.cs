using System.ComponentModel.DataAnnotations;

namespace PollingTask.Presentation.ViewModels
{
    public class CreatePollViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { set; get; }
       
    }
}
