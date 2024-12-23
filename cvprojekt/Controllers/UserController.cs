//using cvprojekt.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Diagnostics;
//using System.Security.Claims;

//namespace cvprojekt.Controllers
//{
//    public class UserController : Controller
//    {
//        private readonly CvDbContext _ctx;
//        private readonly UserManager<User> _userManager;

//        public UserController(CvDbContext ctx, UserManager<User> userManager)
//        {
//            _ctx = ctx;
//            _userManager = userManager;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        [Authorize]
//        [HttpGet]
//        public IActionResult Edit()
//        {
//            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            User user = _ctx.Users.Find(userid);

//            var model = new EditUserViewModel
//            {
//                Name = user.Name,
//                Email = user.Email,
//                IsPrivate = user.IsPrivate,
//                IsActive = user.IsActive,
//            };
                

//            return View(model);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Edit(EditUserViewModel evm)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(evm);
//            }
//            var user = await _userManager.GetUserAsync(User);

//            var result = await _userManager.ChangePasswordAsync(user, evm.CurrentPassword, evm.NewPassword);

//            user.Name = evm.Name;
//            user.Email = evm.Email;
//            user.IsPrivate = evm.IsPrivate;
//            user.IsActive = evm.IsActive;
//            _ctx.SaveChanges();

//            if (result.Succeeded)
//            {
//                return RedirectToAction("Index", "Home");
//            }

//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError(string.Empty, error.Description);
//            }

//            return View(evm);
//        }

//        [HttpGet]
//        public IActionResult Search(string searchWord)
//        {
//            List<User> users = (from user in _ctx.Users select user).Include(u => u.Cvs)
//                .ThenInclude(c => c.Educations)
//                    .ThenInclude(e => e.Skills).ToList();
//            if (!searchWord.IsNullOrEmpty())
//            {
//                string[] searchWords = searchWord.Split(' ');
//                users = users.Where(u => searchWords.Contains(u.Name) || u.Cvs.Any(cv => cv.Educations
//                .Any(edu => edu.Skills
//                    .Any(sid => searchWords.Any(word => sid.Name.Contains(word)))))).ToList();
//            }
            

//            return View(users);
//        }

//    }
//}
