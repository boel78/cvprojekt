
using cvprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cvprojekt.Controllers
{
    public class ProjectController : Controller
    {
        private readonly CvDbContext _context;

        public ProjectController(CvDbContext context)
        {
            _context = context;
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
            return View();

        }
    }
}
