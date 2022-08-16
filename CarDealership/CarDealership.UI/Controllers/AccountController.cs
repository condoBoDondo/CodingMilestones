using CarDealership.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CarDealership.UI.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            var model = new LoginViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // ensure that logging on only works if on the same machine that opened login form
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) // make sure entered data was valid
                return View(model);

            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<AppUser>>();
            var authManager = HttpContext.GetOwinContext().Authentication;

            // attempt loading user
            AppUser user = userManager.Find(model.UserName, model.Password);

            if (user == null)
            {
                // invalid login: no user found
                ModelState.AddModelError("", "Invalid UserName or Password.");
                return View(model);
            }
            else if (userManager.IsInRole(user.Id, "disabled"))
            {
                // invalid login: user disabled
                ModelState.AddModelError("", "Specified User has been disabled.");
                return View(model);
            }
            else 
            {
                // successful login
                // set up cookies
                var identity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new AuthenticationProperties { }, identity);

                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel();

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // using async method as there are some really handy async functions for changing passwords
            if (!ModelState.IsValid)
                return View(model);

            // getting user manager and current user
            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<AppUser>>();
            var user = User.Identity.GetUserId();

            // test changing password; gets current and new password
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                // current password didn't match
                ModelState.AddModelError("", "Current Password was incorrect.");
                return View(model);
            }

            // success
            return RedirectToAction("Index", "Home");
        }
    }
}