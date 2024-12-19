using cvprojekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace cvprojekt.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<Userr> userManager;
        private SignInManager<Userr> signInManager;

        public AccountController(UserManager<Userr> userManager, SignInManager<Userr> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            LoginViewModel vm = new LoginViewModel();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel lm)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(lm.UserName, lm.Password, isPersistent: lm.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(lm);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel rm)
        {
            if (ModelState.IsValid)
            {
                Userr user = new Userr();
                user.UserName = rm.UserName;
                var result = userManager.CreateAsync(user, rm.Password);

                if (result.IsCompletedSuccessfully)
                {
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(rm);
        }


        //Måste göras som en post metod sen i form asp-method
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
