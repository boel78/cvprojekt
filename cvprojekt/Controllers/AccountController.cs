﻿using cvprojekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using cvprojekt.Services;
using Models;

namespace cvprojekt.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private MessageService messageService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            MessageService messageService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> LogIn()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await messageService.GetUnreadMessagesCountAsync(userId) : 0;
            LoginViewModel vm = new LoginViewModel();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel lm)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(lm.UserName, lm.Password,
                    isPersistent: lm.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Felaktigt användarnamn eller lösenord.");
            }

            return View(lm);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await messageService.GetUnreadMessagesCountAsync(userId) : 0;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel rm)
        {
            if (ModelState.IsValid)
            {
                //Sätter default värden på properties som inte finns i IdentityUser
                User user = new User();
                user.UserName = rm.UserName;
                user.Name = rm.Name;
                user.IsPrivate = false;
                user.IsActive = true;
                user.ProfilePicture = [];
                user.Email = rm.Email;
                user.CreatedDate = DateTime.Now;
                var result = await userManager.CreateAsync(user, rm.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Om det inte lyckades, lägg till felmeddelanden till ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine("FEL " + error.ErrorMessage);
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


        [HttpPost]
        public async Task<IActionResult> ChangePassword(EditUserChangePasswordViewModel vm)
        {
            ChangePasswordViewModel evm = vm.changePasswordViewModel;
            ModelState.Remove("EditUserViewModel.Name");
            ModelState.Remove("EditUserViewModel.Email");
            
            var user = await userManager.GetUserAsync(User);

            Console.WriteLine(evm.CurrentPassword);
            if (!string.IsNullOrEmpty(evm.NewPassword) &&
                !string.IsNullOrEmpty(evm.CurrentPassword))
            {
                var result = await userManager.ChangePasswordAsync(user, evm.CurrentPassword,
                    evm.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View("~/Views/User/Edit.cshtml", vm); // Returnera formuläret med felmeddelanden
                }
                TempData["SuccessMessage"] = "Dina ändringar har sparats.";
            }
            else
            {
                return RedirectToAction("Edit", "User");
            }
            
            return RedirectToAction("Edit", "User");

        }
    }
}
