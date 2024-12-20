
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
            var projects = await _context.Projects
                .Include(p => p.User)
                .ToListAsync();

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
    }
}
