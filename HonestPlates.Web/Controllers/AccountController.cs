using Microsoft.AspNetCore.Mvc;
using HonestPlates.Data.Models;
using HonestPlates.Web.Models;

namespace HonestPlates.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET /Account/Register: shows the empty form
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Questions = UserAccount.QuestionChoices;
            return View(new RegisterForm());
        }

        // POST /Account/Register: the form was submitted.
        [HttpPost]
        public IActionResult Register (RegisterForm form)
        {
            // Rules the attributes can't check
            if (form.UserType != "Reviewer" && form.UserType != "Representative")
            {
                ModelState.AddModelError("UserType", "Choose Reviewer or Restaurant Representative");
            }

            if (form.Question1 != null && form.Question2 != null && form.Question3 != null)
            {
                if (Array.IndexOf(UserAccount.QuestionChoices, form.Question1) < 0 ||
                    Array.IndexOf(UserAccount.QuestionChoices, form.Question2) < 0 ||
                    Array.IndexOf(UserAccount.QuestionChoices, form.Question3) < 0)
                {
                    ModelState.AddModelError("Question1", "Pick your questions from the lists.");
                } else if (form.Question1 == form.Question2 || form.Question1 == form.Question3 ||
                    form.Question2 == form.Question3)
                {
                    ModelState.AddModelError("Question3", "Pick 3 different security questions");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    UserAccount account = new UserAccount();
                    string username = form.Username.Trim();

                    if (account.UsernameExists(username))
                    {
                        ModelState.AddModelError("Username", "That username is taken. Please choose another.");
                    }
                    else
                    {
                        account.Username = username;
                        account.Password = form.Password;
                        account.FirstName = form.FirstName.Trim();
                        account.LastName = form.LastName.Trim();
                        account.Email = form.Email.Trim();
                        account.Phone = form.Phone == null ? "" : form.Phone.Trim();
                        account.UserType = form.UserType;
                        account.Question1 = form.Question1;
                        account.Answer1 = form.Answer1.Trim();
                        account.Question2 = form.Question2;
                        account.Answer2 = form.Answer2.Trim();
                        account.Question3 = form.Question3;
                        account.Answer3 = form.Answer3.Trim();

                        if (account.Register())
                        {
                            // "Remember my username": keep it for 30 days, or remove an old one."
                            if (form.RememberUsername)
                            {
                                CookieOptions options = new CookieOptions();
                                options.Expires = DateTime.Now.AddDays(30);
                                Response.Cookies.Append("HP_Username", username, options);
                            }
                            else
                            {
                                Response.Cookies.Delete("HP_Username");
                            }

                            TempData["Message"] = "Your account was created. You can log in now.";
                            return RedirectToAction("Login");
                        }
                        ModelState.AddModelError("","We couldn't create your account. Please try again.");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "We couldn't create your account. Please try again later.");
                }
            }

            ViewBag.Questions = UserAccount.QuestionChoices;
            return View(form);
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
