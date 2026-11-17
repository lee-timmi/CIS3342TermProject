using Microsoft.AspNetCore.Mvc;
using HonestPlates.Data.Models;
using HonestPlates.Web.Models;
using HonestPlates.Data;

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

        // GET /Account/Verify: from the email link (username + code) or after registering (username only)
        [HttpGet]
        public IActionResult Verify(string username, string code)
        {
            VerifyForm form = new VerifyForm();
            form.Username = username;
            form.Code = code;

            // Clicking the email link verifies right away
            // Came from Murach's ASP.NET Core MVC, Chapter 9 Page 331 on string.IsNullOrEmpty checks.
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(code))
            {
                return CheckCode(form);
            }
            return View(form);
        }

        [HttpPost]
        public IActionResult Verify(VerifyForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }
            return CheckCode(form);
        }


        // Shared by both: checks the code and marks the account verified
        // Came from Murach's ASP.NET Core MVC, Chapter 11 Page 407 on setting validation messages in code.
        private IActionResult CheckCode(VerifyForm form)
        {
            try
            {
                UserAccount account = new UserAccount();
                if (account.Verify(form.Username.Trim(), form.Code.Trim()))
                {
                    TempData["Message"] = "Your account is verified. You can log in now.";
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "That code doesn't match, or this account is already verified.");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "We couldn't verify your account right now. Please try again later.");
            }
            return View("Verify", form);
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
                        account.MakeVerificationCode();

                        if (account.Register())
                        {
                            // "Remember my username": keep it for 30 days, or remove an old one."
                            if (form.RememberUsername)
                            {
                                CookieOptions options = new CookieOptions();
                                options.Expires = DateTime.Now.AddDays(30);
                                Response.Cookies.Append("HP_Username", Encryption.Encrypt(username), options);
                            }
                            else
                            {
                                Response.Cookies.Delete("HP_Username");
                            }

                            // The link opens the Verify page with the username and code filled in.
                            // Standard ASP.NET Core (Url.Action with a protocol makes a full link); not in the textbooks.
                            string verifyLink = Url.Action("Verify", "Account", new { username = username, code = account.VerificationCode }, Request.Scheme);

                            try
                            {
                                account.SendVerificationEmail(verifyLink);
                                TempData["Message"] = "Your account was created! We emailed a 6-digit code to " + account.Email + ".";
                            } 
                            catch (Exception)
                            {
                                // The account is saved either way; only the email failed
                                TempData["Message"] = "Your account was created, but we couldn't send the email. Please contact us to verify your account.";
                            }

                            // Came from Murach's ASP.NET Core MVC, Chapter 8 Page 303 on redirecting with route values.
                            return RedirectToAction("Verify", new { username = username });

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
        
        [HttpGet]       
        public IActionResult Login()
        {
            LoginForm form = new LoginForm();

            // The remembered username is saved encrypted, so decrypt it.
            string? savedCookie = Request.Cookies["HP_Username"];

            if (savedCookie != null)
            {
                try
                {
                    form.Username = Encryption.Decrypt(savedCookie);
                    form.RememberMe = true;
                }
                catch (Exception)
                {
                    Response.Cookies.Delete("HP_Username");
                }
            }

            return View(form);
        }

        // POST /Account/Login: checks the username and password, then logs the user in
        // Came from Murach's ASP.NET Core MVC, Chapter 16 Page 679 on the LogIn() action for POST requests.
        [HttpPost]
        public IActionResult Login(LoginForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            try
            {
                UserAccount account = new UserAccount();
                string username = form.Username.Trim();

                if (!account.Login(username, form.Password))
                {
                    ModelState.AddModelError("", "Wrong username or password.");
                    return View(form);
                }

                if (!account.IsVerified)
                {
                    ViewBag.VerifyUsername = username;
                    ModelState.AddModelError("", "Please verify your account first. Check your email for the 6-digit code.");
                    return View(form);
                }

                // Logged in: save who they are
                // Came from Murach's ASP.NET Core MVC, Chapter 9 Page 327 on working with session state.
                HttpContext.Session.SetInt32("UserID", account.UserID);
                HttpContext.Session.SetString("FirstName", account.FirstName);
                HttpContext.Session.SetString("UserType", account.UserType);

                // "Remember me": the username is saved ENCRYPTED in the cookie (Tri's "encrypt the login cookie" item)
                // Came from Murach's ASP.NET Core MVC, Chapter 9 Page 349 on working with cookies.
                if (form.RememberMe)
                {
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(30);
                    options.HttpOnly = true;
                    Response.Cookies.Append("HP_Username", Encryption.Encrypt(username), options);
                }
                else
                {
                    Response.Cookies.Delete("HP_Username");
                }

                return RedirectToAction("Dashboard");
            } catch (Exception)
            {
                ModelState.AddModelError("", "We couldn't log you in right now. Please try again later.");
            }
            return View(form);
        }

        [HttpPost]
        public IActionResult ForgetMe()
        {
            Response.Cookies.Delete("HP_Username");
            TempData["Message"] = "This computer no longer remembers your username.";
            return RedirectToAction("Login");
        }


        // Logs the user out: removes who they are from Session, then goes back to the home page.
        // Came from Murach's ASP.NET Core MVC, Chapter 9 Page 327 on working with session state (Remove).
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("FirstName");
            HttpContext.Session.Remove("UserType");
            return RedirectToAction("Index", "Home");
        }

        // GET /Account/Dashboard: the logged-in user's home page (both user types).
        // Logged-out visitors are sent to Log In instead (they can't open it by typing the URL).
        // Came from Murach's ASP.NET Core MVC, Chapter 9 Page 327 on working with session state.
        public IActionResult Dashboard()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
            {
                TempData["Message"] = "Please log in to see your dashboard.";
                return RedirectToAction("Login");
            }

            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.UserType = HttpContext.Session.GetString("UserType");

            // Representatives can review other restaurants
            try
            {
                Review review = new Review();
                ViewBag.MyReviews = review.GetReviewsByUser(userID.Value);
            }
            catch (Exception)
            {
                ViewBag.ReviewsError = "Unable to load your reviews right now. Please try again later.";
            }

            return View();
        }
    }
}
