using System.ComponentModel.DataAnnotations;

namespace PollingTask.Presentation.ViewModels
{
	public class AdminLoginView
	{
		[Required]
		[EmailAddress]
		public string Email { set; get; }

		[Required]
		[DataType(DataType.Password)]
		public string? Password { set; get; }

		[Display(Name = "Remember Me")]
		public bool RememberMe { set; get; }

		//[AllowNull]
		public string? ReturnUrl { set; get; }
	}
}
