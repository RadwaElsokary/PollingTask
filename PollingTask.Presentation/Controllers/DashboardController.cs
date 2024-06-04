using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PollingTask.Domain.Model;
using PollingTask.Presentation.Models;
using PollingTask.Presentation.ViewModels;
using PollingTask.Repository;
using PollingTask.Repository.IRepsitory;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace PollingTask.Presentation.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly ApplicationDbContext context;

        public DashboardController(IUnitOfWork unitOfWork, ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.unitOfWork = unitOfWork;
            this.context = context;
        }

		public IActionResult DashboardHome()
		{
			return View();
		}

		[HttpGet]
        [AllowAnonymous]
        public IActionResult AdminLogin(string? returnUrl)
        {
            AdminLoginView model = new AdminLoginView
            {
                ReturnUrl = returnUrl,
            };
            return View(model);  
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin(AdminLoginView model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                    if (userRole != null && userRole == "Admin")
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
                                return RedirectToAction("DashboardHome", "Dashboard");  // Redirect to the home page or dashboard
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
            // If we got this far, something failed; redisplay the form with errors
            return View(model);
        }

        public IActionResult GetAllPolls()
        {
            return View();
        }


        [HttpGet]
        public IActionResult GetAllPollsApi()
        {
            var result =  unitOfWork.Repository<Poll>().GetAll().ToList();
            return Json(result);
        }


        public IActionResult CreatePoll()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreatePollPost([FromForm] CreatePollViewModel model)
		{
			var poll = new Poll
			{
				Title = model.Title
			};
			await unitOfWork.Repository<Poll>().Add(poll);
			await unitOfWork.Complete();

			return RedirectToAction("GetAllPolls"); // Redirect to the appropriate action after creating the poll
		}

       

        [HttpGet]
        public ViewResult UpdatePoll(int Id)
        {
            Poll poll = unitOfWork.Repository<Poll>().GetById(Id).Result;
            CreatePollViewModel pollView = new CreatePollViewModel
            {
                Id = poll.Id,
                Title = poll.Title
            };
            return View(pollView);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePoll(CreatePollViewModel model)
        {
            if (ModelState.IsValid)
            {
                Poll poll = unitOfWork.Repository<Poll>().GetById(model.Id).Result;
                poll.Title = model.Title;

                await unitOfWork.Repository<Poll>().Update(poll);
                await unitOfWork.Complete();

                return RedirectToAction("GetAllPolls");
            }
            return View();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePoll(int Id)
        {
            if (ModelState.IsValid)
            {
                Poll poll = unitOfWork.Repository<Poll>().GetById(Id).Result;
                var questions = unitOfWork.Repository<Question>().GetAll().Where(p => p.PollId == poll.Id).ToList();
                foreach(var question in questions)
                {
                    await unitOfWork.Repository<Question>().Delete(question);

                }
                await unitOfWork.Repository<Poll>().Delete(poll);
                await unitOfWork.Complete();

                return RedirectToAction("GetAllPolls");
            }
            return View();
        }

		public IActionResult GetAllQuestions()
		{
			return View();
		}

		[HttpGet]
		public IActionResult GetAllQuestionsApi()
		{
			var result = unitOfWork.Repository<Question>().GetAll().ToList();
			return Json(result);
		}

        public IActionResult CreateQuestion()
        {
            var model = new CreateQuestionViewModel();
            model.QuestionId = 1; // Set the default value for QuestionId
            model.PollId = 1; // Set the default value for PollId

            // Populate poll IDs (assuming you have a method to get them from your data source)
            var pollIds = unitOfWork.Repository<Poll>().GetAll().Select(p => p.Id).ToList();
            ViewBag.PollIds = new SelectList(pollIds);

            model.Answers.Add(new AnswerViewModel()); // Initially add one answer
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestionPost(CreateQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save question to database
                var question = new Question
                {
                    Text = model.QuestionText,
                    PollId =model.PollId
                };
                await unitOfWork.Repository<Question>().Add(question);
                await unitOfWork.Complete();

                // Save answers to database
                foreach (var answer in model.Answers)
                {
                    answer.QuestionId = question.Id;
                    var newAnswer = new Answer
                    {
                        Text = answer.AnswerText,
                        QuestionId = question.Id
                    };
                    await unitOfWork.Repository<Answer>().Add(newAnswer);
                }
                await unitOfWork.Complete();

                return RedirectToAction("GetAllQuestions"); // Redirect to appropriate action
            }
            // If model state is not valid, return the view with errors
            return View("CreateQuestion", model);
        }

        [HttpGet]
        public ViewResult UpdateQuestion(int id)
        {
            // Get the question by ID
            var question = unitOfWork.Repository<Question>().GetById(id).Result;

            // Create a view model to hold the data
            var model = new CreateQuestionViewModel
            {
                QuestionId = question.Id,
                QuestionText = question.Text,
                PollId = question.PollId // Assuming there's a PollId property in the Question model
            };

            // Populate poll IDs (assuming you have a method to get them from your data source)
            var pollIds = unitOfWork.Repository<Poll>().GetAll().Select(p => p.Id).ToList();
            ViewBag.PollIds = new SelectList(pollIds, model.PollId); // Set the selected value to the current question's poll ID

            // Populate answers for the question
            var answers = unitOfWork.Repository<Answer>().GetAll().Where(a => a.QuestionId == id).ToList();
            foreach (var answer in answers)
            {
                model.Answers.Add(new AnswerViewModel
                {
                  //  QuestionId = answer.Id,
                    AnswerText = answer.Text
                });
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuestion(CreateQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var question = await unitOfWork.Repository<Question>().GetById(model.QuestionId);
                if (question == null)
                {
                    return NotFound();
                }

                // Update the question data
                question.Text = model.QuestionText;
                question.PollId = model.PollId;

                // Ensure question.Answers is not null
                if (question.Answers == null)
                {
                    question.Answers = new List<Answer>();
                }

                // Process each answer in the model
                foreach (var answerViewModel in model.Answers)
                {
                    var existingAnswer = question.Answers.FirstOrDefault(a => a.Id == answerViewModel.Id);

                    if (existingAnswer != null)
                    {
                        // Update existing answer text
                        existingAnswer.Text = answerViewModel.AnswerText;
                    }
                    else
                    {
                        // Add new answer
                        var newAnswer = new Answer
                        {
                            Text = answerViewModel.AnswerText,
                            QuestionId = question.Id
                        };
                        question.Answers.Add(newAnswer);
                    }
                }

                await unitOfWork.Complete();

                return RedirectToAction("GetAllQuestions");
            }

            // If model state is not valid, return the view with errors
            return View(model);
        }

        [HttpGet]
        public ViewResult ViewQuestion(int id)
        {
            // Get the question by ID
            var question = unitOfWork.Repository<Question>().GetById(id).Result;
            var poll = unitOfWork.Repository<Poll>().GetById(question.PollId).Result ;
            // Create a view model to hold the data
            var model = new CreateQuestionViewModel
            {
                QuestionId = question.Id,
                QuestionText = question.Text,
                Poll = poll == null ? null : poll.Title  // Assuming there's a PollId property in the Question model
            };

            // Populate poll IDs (assuming you have a method to get them from your data source)
            var pollIds = unitOfWork.Repository<Poll>().GetAll().Select(p => p.Id).ToList();
            ViewBag.PollIds = new SelectList(pollIds, model.PollId); // Set the selected value to the current question's poll ID

            // Populate answers for the question
            var answers = unitOfWork.Repository<Answer>().GetAll().Where(a => a.QuestionId == id).ToList();
            foreach (var answer in answers)
            {
                model.Answers.Add(new AnswerViewModel
                {
                    //  QuestionId = answer.Id,
                    AnswerText = answer.Text
                });
            }

            return View(model);
        }

        [HttpGet]
        public ViewResult ViewPoll(int id)
        {
            // Get the poll by ID
            var poll = unitOfWork.Repository<Poll>().GetById(id).Result;

            // Get questions related to the poll
            var questions = unitOfWork.Repository<Question>().GetAll().Where(q => q.PollId == id).ToList();

            // Create a view model to hold the data
            var model = new ViewPollViewModel
            {
                PollId = poll.Id,
                PollText = poll.Title,
                Questions = questions,
            };

            return View(model);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteQuestion(int Id)
        {
            if (ModelState.IsValid)
            {
                Question question = unitOfWork.Repository<Question>().GetById(Id).Result;
                await unitOfWork.Repository<Question>().Delete(question);
                await unitOfWork.Complete();

                return RedirectToAction("GetAllQuestions");
            }
            return View();
        }


        [HttpGet]
        public IActionResult GetAllUserAnswers()
        {
            
            var userAnswers = unitOfWork.Repository<UserAnswer>().GetAll().ToList();
            
            var userAnswerViewModels = userAnswers.Select(ua => new UserAnswerViewModel
            {
                UserEmail = userManager.Users.FirstOrDefaultAsync(user => user.Id == ua.UserId).Result.Email, 
                PollText = GetPoll(ua.QuestionId) == null ? null : GetPoll(ua.QuestionId).Title, 
                QuestionText = unitOfWork.Repository<Question>().GetById(ua.QuestionId).Result == null  ? null : unitOfWork.Repository<Question>().GetById(ua.QuestionId).Result.Text,
                AnswerText = unitOfWork.Repository<Answer>().GetById(ua.AnswerId).Result == null ? null : unitOfWork.Repository<Answer>().GetById(ua.AnswerId).Result.Text,
            }).ToList();
            return View(userAnswerViewModels);
        }

        private Poll GetPoll(int QuestionId)
        {
            var pollId = unitOfWork.Repository<Question>().GetAll().FirstOrDefault(question => question.Id == QuestionId).PollId;
           Poll poll = unitOfWork.Repository<Poll>().GetById(pollId).Result;
            return poll;
        }




    }
}
