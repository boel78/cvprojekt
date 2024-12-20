using cvprojekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace cvprojekt.Controllers
{
    public class CVController : Controller
    {
        private readonly CvDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public CVController(CvDbContext dbContext, UserManager<User> userManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }


        public IActionResult Index(string projekt)
        {
            List<Cv> cvs = _dbContext.Cvs.Where(c => c.Projects.Any(p => p.Title.Contains(projekt))).ToList();

            return View(cvs);
        }

        [HttpGet]
        public IActionResult AddEducation()
        {
            List<SelectListItem> skills = _dbContext.Skills.Select(x => new SelectListItem { Text = x.Name, Value = x.Sid.ToString() }).ToList();

            ViewBag.options = skills;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEducation(Education education)
        {
            string userid = _userManager.GetUserId(User);

            var user = _dbContext.Users.Where(u => u.Id == userid).Include(u => u.Cvs).FirstOrDefault();

            
            education.Cvid = user.Cvs.FirstOrDefault().Cvid;
            _dbContext.Educations.Add(education);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
