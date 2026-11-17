using Microsoft.AspNetCore.Mvc;
using HonestPlates.Data.Models;
using HonestPlates.Web.Models;
using HonestPlates.Data;
using System.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        // -------- FORGOT username / RESET password ----------------

        // Page 1 (GET): What did user forget? /// (POST): find account > random question > page 2
        [HttpGet]
        public IActionResult Forgot(string mode)
        {
            ClearRecovery();
            if (mode != "username" && mode != "password")
            {
                return RedirectToAction("Login");
            }
            ForgotForm form = new ForgotForm();
            form.Mode = mode;
            return View(form);
        }
        
        [HttpPost]
        public IActionResult Forgot(ForgotForm form)
        {
            if (form.Mode != "username" && form.Mode != "password")
            {
                ModelState.AddModelError("Mode", "Choose whether you forgot your username or password.");
            }
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            try
            {
                UserAccount account = new UserAccount();
                string lookup = form.Lookup.Trim();
                bool found;

                if (form.Mode == "username")
                {
                    found = account.FindByEmail(lookup);
                } else
                {
                    found = account.FindByUsername(lookup);
                }

                if (!found)
                {
                    ModelState.AddModelError("", "We couldn't find an account with that " + (form.Mode == "username" ? "email." : "username."));
                    return View(form);
                }

                // Picking one of the user's 3 security questions at random
                DataRow question = account.GetRandomQuestion();
                if (question == null)
                {
                    ModelState.AddModelError("", "This account has no security questions, therefore cannot be recovered. Please contact us.");
                    return View(form);
                }

                // Remember who is recovering and what questions were asked.
                HttpContext.Session.SetInt32("RecoverUserID", account.UserID);
                HttpContext.Session.SetString("RecoverUsername", account.Username);
                HttpContext.Session.SetString("RecoverMode", form.Mode);
                HttpContext.Session.SetInt32("RecoverQuestionID", Convert.ToInt32(question["QuestionID"]));
                HttpContext.Session.SetString("RecoverQuestionText", question["QuestionText"].ToString());
                return RedirectToAction("Question");

            } catch (Exception)
            {
                ModelState.AddModelError("", "We couldn't look up your account right now. Please try again later.");
            }

            return View(form);
        }

        // Page 2 (GET): Show random question // (POST): Check answer
        [HttpGet]
        public IActionResult Question()
        {
            if (HttpContext.Session.GetInt32("RecoverUserID") == null)
            {
                return RedirectToAction("Forgot");
            }

            QuestionForm form = new QuestionForm();
            form.QuestionText = HttpContext.Session.GetString("RecoverQuestionText");
            return View(form);
        }

        [HttpPost]
        public IActionResult Question(QuestionForm form)
        {
            if (HttpContext.Session.GetInt32("RecoverUserID") == null || HttpContext.Session.GetInt32("RecoverQuestionID") == null)
            {
                return RedirectToAction("Forgot");
            }
            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("RecoverUserID"));
            int questionID = Convert.ToInt32(HttpContext.Session.GetInt32("RecoverQuestionID"));
            form.QuestionText = HttpContext.Session.GetString("RecoverQuestionText");

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            try
            {
                UserAccount account = new UserAccount();
                account.UserID = userID;
                if (!account.CheckQuestionAnswer(questionID, form.Answer))
                {
                    ModelState.AddModelError("", "That answer doesn't match. Try again.");
                    return View(form);
                }

                if (HttpContext.Session.GetString("RecoverMode") == "username")
                {
                    // forgot username; show on the login page, then forget the recovery
                    ViewBag.Username = HttpContext.Session.GetString("RecoverUsername");
                    ClearRecovery();
                    return View("UsernameFound");
                }

                // Forgot password. Page 3.
                HttpContext.Session.SetInt32("RecoverPassed", 1);
                return RedirectToAction("ResetPassword");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "We couldn't check your answer right now. Please try again later.");
            }

            return View(form);
        }

        // Page 3 (GET): choose new password (after correct answer only) // (POST): save new password (encrypted)
        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (HttpContext.Session.GetInt32("RecoverPassed") == null)
            {
                return RedirectToAction("Forgot");
            }
            return View(new ResetPasswordForm());
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordForm form)
        {
            if (HttpContext.Session.GetInt32("RecoverPassed") == null || HttpContext.Session.GetInt32("RecoverUserID") == null)
            {
                return RedirectToAction("Forgot");
            }

            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("RecoverUserID"));

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            try
            {
                UserAccount account = new UserAccount();
                account.UserID = userID;
                if (account.UpdatePassword(form.NewPassword))
                {
                    ClearRecovery();
                    TempData["Message"] = "Your password was changed. You can log in now.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "We couldn't change your password. Please try again.");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "We couldn't change your password right now. Please try again later.");
            }

            return View(form);
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
        public IActionResult Login(string username)
        {
            LoginForm form = new LoginForm();
            if (username != null)
            {
                form.Username = username;
                return View(form);
            }

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

        // Forgets everything about an account recovery in progress
        private void ClearRecovery()
        {
            HttpContext.Session.Remove("RecoverUserID");
            HttpContext.Session.Remove("RecoverUsername");
            HttpContext.Session.Remove("RecoverMode");
            HttpContext.Session.Remove("RecoverQuestionID");
            HttpContext.Session.Remove("RecoverQuestionText");
            HttpContext.Session.Remove("RecoverPassed");
        }
    }
}
