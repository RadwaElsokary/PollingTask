using Microsoft.AspNetCore.Mvc;
using PollingTask.Presentation.Utilities;
using System.ComponentModel.DataAnnotations;

namespace PollingTask.Presentation.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[EmailAddress]
		[Remote(action: "IsEmailUse", controller: "Account")]
		[ValidEmailDomain(allowedDomain: "email.com", ErrorMessage = "Your email should have email.com")]
		public string Email { set; get; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { set; get; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm Pssword")]
		[Compare("Password", ErrorMessage = "Password and confirm password do not match")]
		public string ConfirmPassword { set; get; }

	}
}
