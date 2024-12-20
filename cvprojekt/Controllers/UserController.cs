using cvprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace cvprojekt.Controllers
{
    public class UserController : Controller
    {
        private readonly CvDbContext _ctx;

        public UserController(CvDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            User user = _ctx.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(int id, User newUser)
        {
            User oldUser = _ctx.Users.Find(id);

            oldUser.Name = newUser.Name;
            oldUser.Email = newUser.Email;
            //oldUser.Password = newUser.Password;
            oldUser.IsPrivate = newUser.IsPrivate;
            oldUser.IsActive = newUser.IsActive;

            _ctx.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Search(string searchWord)
        {
            List<User> users = (from user in _ctx.Users select user).Include(u => u.Cvs)
                .ThenInclude(c => c.Educations)
                    .ThenInclude(e => e.Sids).ToList();
            if (!searchWord.IsNullOrEmpty())
            {
                string[] searchWords = searchWord.Split(' ');
                users = users.Where(u => searchWords.Contains(u.Name) || u.Cvs.Any(cv => cv.Educations
                .Any(edu => edu.Sids
                    .Any(sid => searchWords.Any(word => sid.Name.Contains(word)))))).ToList();
            }
            

            return View(users);
        }

    }
}
