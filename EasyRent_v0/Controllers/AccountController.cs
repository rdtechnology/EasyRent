using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EasyRent_v0.Models;
using EasyRental.Database;
using System.Web.Security;

namespace EasyRent_v0.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            using (var db = SiteUtil.NewDb)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        // Remove whitespace from username - Apache 2015-11-23
                        var userName = model.Email.Trim().ToLower();
                        var user = db.Users.Where(n => n.UserName == userName && n.IsActive.Value).FirstOrDefault();

                        if (ValidateUser(user, userName, model.Password))
                        {
                            FormsAuthentication.SetAuthCookie(model.Email.ToLower(), model.RememberMe);

                            //ResetCurrentUserSession(model.Email);
                            SiteUtil.CurrentUser = user;
                            return RedirectToLocal(returnUrl);

                        }
                        else
                        {
                            ModelState.AddModelError("", "The Email or Password provided is incorrect.");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The Email or Password provided is incorrect.");
                        return View(model);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    return View(model);
                }
            } 
        }

        public ActionResult LogOff()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Abandon();
            FormsAuthentication.SignOut();
            SiteUtil.CurrentUser = null;
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewUser(RegisterViewModel model)
        {
            try
            {
                using (var db = SiteUtil.NewDb)
                {
                    var emailConfirmToken = SiteUtil.GetToken(model.Email);
                    var newUser = new User()
                    {
                        Email = model.Email,
                        Password = model.Password,
                        UserName = model.Email,
                        RoleId = db.Lookups.Where(x => x.Description == UserType.Renter).FirstOrDefault().Id,
                        CreatedOn = DateTime.Now,
                        CreatedBy = model.Email,
                        UpdatedBy = model.Email,
                        UpdatedOn = DateTime.Now,
                        EmailConfirmToken = emailConfirmToken,
                        IsActive = true,
                        CanLogin = false
                    };

                    var newContact = new Contact()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        CreatedBy = model.Email,
                        UpdatedBy = model.Email,
                        UpdatedOn = DateTime.Now,
                    };

                    newUser.Contacts.Add(newContact);
                    db.Users.Add(newUser);
                    db.SaveChanges();

                    string Subject = "Welcome to EasyRental - AIMY";
                    string Body = string.Format("Thank you for sign up, please click on the link to activate your account within 24 hours : <a href=\"{0}\" title=\"Account Activation \">Click here</a> ", Url.Action("AccountActivation", "Account", new { Token = emailConfirmToken, Email = model.Email }, Request.Url.Scheme));

                    return RedirectToAction("RegisterConfirmation", "Account");
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public ActionResult RegisterConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public ActionResult AccountActivation(string token, string email)
        {
            if (token == null || email == null)
            {
                return View("Error");
            }

            using (var db = SiteUtil.NewDb)
            {
                var user = db.Users.Where(x => x.Email == email).FirstOrDefault();

                if (user.EmailConfirmToken != token)
                {
                    return View("Error");
                }
                else
                {
                    return View("Login", "Account");
                }
            }
        }

        public bool ValidateUser(User user, string userName, string password)
        {
            using (var db = SiteUtil.NewDb)
            {
                if (user == null)
                {
                    return false;
                }

                if (!user.CanLogin.Value)
                {
                    throw new Exception("Account has been disabled, please contact support.");
                }

                // PASSWORDHASH:
                // Compared User's hashed password with incoming password hased by the hashkey which used to hash the password for user at the first place.
                if ((user.Password != null && user.Password == password)) { 
                    //SiteUtil.CreatePasswordHash(password, user.PasswordHash))){
                    if (user.RetryAttempt < 4 && user.CanLogin.Value)
                    {
                        user.RetryAttempt = 0;
                    }

                    //user.LastLogedOn = DateTime.Now;
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    // increment retry attempts
                    if (user.RetryAttempt < 4 || !user.RetryAttempt.HasValue)
                    {
                        user.RetryAttempt = user.RetryAttempt.GetValueOrDefault() + 1;
                    }
                    else
                    {
                        user.CanLogin = false;
                    }

                    db.SaveChanges();
                    return false;
                }
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (returnUrl == null || returnUrl == "")
            {
                return RedirectToAction("Index", "Home");
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await UserManager.FindByNameAsync(model.Email);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return View("ForgotPasswordConfirmation");
                //}

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            //var user = await UserManager.FindByNameAsync(model.Email);
            //if (user == null)
            //{
            //    // Don't reveal that the user does not exist
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}
            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            //if (result.Succeeded)
            //{
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}
            //AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

    }
}