using cvprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace cvprojekt.Controllers
{
    public class UserController : Controller
    {
        private readonly CvDbContext _ctx;

        public UserController(CvDbContext ctx) {
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
            oldUser.Password = newUser.Password;
            oldUser.IsPrivate = newUser.IsPrivate;
            oldUser.IsActive = newUser.IsActive;

            _ctx.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Search(string searchWord)
        {
            Debug.WriteLine("sök: " + searchWord);
            List<User> users = (from user in _ctx.Users where user.Name.Contains(searchWord) select user).ToList();
            return View(users);
        }

    }
}
