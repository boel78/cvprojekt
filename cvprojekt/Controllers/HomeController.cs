using System.Diagnostics;

using cvprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cvprojekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CvDbContext _context;

        public HomeController(ILogger<HomeController> logger, CvDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(bool showOnlyActive)
        {
            IQueryable<User> userList = from user in _context.Users select user;
            if (showOnlyActive)
            {
                userList = userList.Where(x => x.IsActive);
            }
            else
            {
                userList = userList.Where(x => !x.IsActive);
            }
            ViewData["ShowOnlyActive"] = showOnlyActive;
            return View(userList.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
