
using cvprojekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cvprojekt.Controllers
{
    public class ProjectController : Controller
    {
        private readonly CvDbContext _context;
        private readonly UserManager<User> _usermanager;
        public ProjectController(CvDbContext context, UserManager<User> usermanager)
        {
            _context = context;
            _usermanager = usermanager; 
        }

        public async Task<IActionResult> Index()
        {
            IQueryable<Project> projects =  (from project in _context.Projects
                                            select project).Include(p => p.CreatedByNavigation);

            return View(projects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Project project)
        {
                string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                project.CreatedBy = userid;
                _context.Add(project);
                _context.SaveChanges();
                return RedirectToAction("Index", "Project");
        }

        [HttpGet]
        public IActionResult Remove(int id)
        {
            Project project = _context.Projects.Find(id);
            _context.Projects.Remove(project);
            _context.SaveChanges();
            return RedirectToAction("Index", "Project");

        }
    }
}
