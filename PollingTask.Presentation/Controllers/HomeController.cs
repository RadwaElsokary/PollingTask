using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PollingTask.Domain.Model;
using PollingTask.Presentation.Models;
using PollingTask.Presentation.ViewModels;
using PollingTask.Repository.IRepsitory;
using PollingTask.Repository.Repsitory;
using System.Diagnostics;

namespace PollingTask.Presentation.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<IdentityUser> userManager; // Assuming ApplicationUser is your user entity
		private readonly SignInManager<IdentityUser> signInManager;
        private readonly IUnitOfWork unitOfWork;

		public HomeController(IUnitOfWork unitOfWork,ILogger<HomeController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
		{
			_logger = logger;
			this.userManager = userManager;
			this.signInManager = signInManager;
            this.unitOfWork = unitOfWork;
		}

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }

        [HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string? returnUrl)
		{
			LoginView model = new LoginView
			{
				ReturnUrl = returnUrl,
			};
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginView model, string? returnUrl = null)
		{
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                    if (userRole != null && userRole == "User")
                    {
                        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return LocalRedirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("GetLastQuestion", "Home");  // Redirect to the home page or dashboard
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }
            return View(model);
		}

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password);
                await userManager.AddToRoleAsync(user, "User");
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "You registered successfully.";
                    return RedirectToAction("Login", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }


        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already use");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetLastQuestion()
        {
            var lastQuestion = unitOfWork.Repository<Question>()
                                       .GetAll()
                                       .OrderByDescending(q => q.Id)
                                       .FirstOrDefault();

            if (lastQuestion == null)
            {
                return NotFound("No questions found.");
            }

            var userId = userManager.GetUserId(User); // Get the current logged-in user's ID

            var answered  = unitOfWork.Repository<UserAnswer>()
                                    .GetAll()
                                    .Where(u =>  u.UserId == userId && u.QuestionId == lastQuestion.Id)
                                    .FirstOrDefault();
            if (answered == null)
            {


                var poll = await unitOfWork.Repository<Poll>().GetById(lastQuestion.PollId);
                var answers = unitOfWork.Repository<Answer>().GetAll()
                                        .Where(a => a.QuestionId == lastQuestion.Id)
                                        .ToList();

                var model = new LastQuestionViewModel
                {
                    QuestionId = lastQuestion.Id,
                    PollText = poll.Title,
                    QuestionText = lastQuestion.Text,
                    Answers = answers.Select(a => new AnswerViewModel { Id = a.Id, AnswerText = a.Text }).ToList()
                };

                return View(model);
            }
            else
            {
                var question = await unitOfWork.Repository<Question>().GetById(answered.QuestionId);
                if (question != null)
                {
                    var poll = await unitOfWork.Repository<Poll>().GetById(question.PollId);
                    var model = new LastQuestionViewModel
                    {
                        PollText = poll != null ? poll.Title : null,
                        QuestionText = question.Text,
                        AnswerText = unitOfWork.Repository<Answer>().GetById(answered.AnswerId).Result?.Text
                    };
                    return View("UserAnswered", model);
                }
                else
                {
                    // Handle the case where the question is not found
                    return NotFound();
                }

            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswer(LastQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            var userId = userManager.GetUserId(User); // Get the current logged-in user's ID

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User must be logged in to submit an answer.");
            }

            var selectedAnswer = await unitOfWork.Repository<Answer>().GetById(model.SelectedAnswerId);
            if (selectedAnswer == null)
            {
                return NotFound("Selected answer not found.");
            }

            // Create a new UserAnswer object
            var userAnswer = new UserAnswer
            {
                UserId = userId,
                QuestionId = model.QuestionId,
                AnswerId = model.SelectedAnswerId
            };

            // Add the new UserAnswer to the database
            await unitOfWork.Repository<UserAnswer>().Add(userAnswer);

            // Save changes to the database
            var result = await unitOfWork.Complete();

            if (result > 0) // Check if the changes were successfully saved
            {
                return RedirectToAction("GetLastQuestion", "Home");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Error saving data.");
        }
        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


	}
}
