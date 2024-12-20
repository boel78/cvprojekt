using cvprojekt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;

namespace cvprojekt.Controllers
{
    public class UserController : Controller
    {
        private readonly CvDbContext _ctx;
        private readonly UserManager<User> _userManager;

        public UserController(CvDbContext ctx, UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _ctx.Users.Find(userid);

            var model = new EditUserViewModel
            {
                Name = user.Name,
                Email = user.Email,
                IsPrivate = user.IsPrivate,
                IsActive = user.IsActive,
            };
                

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel evm)
        {
            if (!ModelState.IsValid)
            {
                return View(evm);
            }
            var user = await _userManager.GetUserAsync(User);

            var result = await _userManager.ChangePasswordAsync(user, evm.CurrentPassword, evm.NewPassword);

            user.Name = evm.Name;
            user.Email = evm.Email;
            user.IsPrivate = evm.IsPrivate;
            user.IsActive = evm.IsActive;
            _ctx.SaveChanges();

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(evm);
        }

        [HttpGet]
        public IActionResult Search(string searchWord)
        {
            List<User> users = new List<User>();

            if (User.Identity.IsAuthenticated)
            {
                users = (from user in _ctx.Users where user.IsActive == true select user).Include(u => u.Cvs)
                .ThenInclude(c => c.Educations)
                    .ThenInclude(e => e.Skills).ToList();

                if (!searchWord.IsNullOrEmpty())
                {
                    string[] searchWords = searchWord.Split(' ');
                    users = users.Where(u => searchWords.Any(word => u.Name.Trim().ToLower().Contains(word.Trim().ToLower())
                          || u.Cvs.Any(cv => cv.Educations
                     .Any(edu => edu.Skills
                         .Any(sid => searchWords.Any(word => sid.Name.Contains(word))))))).ToList();
                }
            }
            else
            {
                
                users = (from user in _ctx.Users where user.IsPrivate == false where user.IsActive == true select user).Include(u => u.Cvs)
                .ThenInclude(c => c.Educations)
                    .ThenInclude(e => e.Skills)
                        .ToList();

                if (!searchWord.IsNullOrEmpty())
                {
                    string[] searchWords = searchWord.Split(' ');
                    
                    foreach (string word in searchWords)
                    {
                        Debug.WriteLine(word);
                    }
                    users = users.Where(u => searchWords.Any(word => u.Name.Trim().ToLower().Contains(word.Trim().ToLower())
                         || u.Cvs.Any(cv => cv.Educations
                    .Any(edu => edu.Skills
                        .Any(sid => searchWords.Any(word => sid.Name.Contains(word))))))).ToList();
                }

            }

            return View(users);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Image()
        {
            User user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Image(IFormFile image)
        {
            User user = await _userManager.GetUserAsync(User);
            if (image != null)
            {
                if (image.Length > 0)
                {
                    Debug.WriteLine("HEJHEJHEJ");
                    byte[] p1 = null;
                    using (var fs1 = image.OpenReadStream())
                    using (var msa1 = new MemoryStream())
                    { 
                        fs1.CopyTo(msa1);
                        p1 = msa1.ToArray();
                    }

                    user.ProfilePicture = p1;

                }
            }
            _ctx.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

    }
}
