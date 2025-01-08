using System.Linq;
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
        //Hämtar en lista med projekt och visar den på indexsidan
        public async Task<IActionResult> Index()
        {
            bool isAuthenticated = User.Identity.IsAuthenticated; // kollar om användaren är inloggad
            ProjectViewModel projectViewModel = new ProjectViewModel(); //skapar viewmodel för att skicka data till vyn
            projectViewModel.User = await _usermanager.GetUserAsync(User);
            
            // Hämtar projekt och får med både skapare av projekten och kopplade användare.
            IQueryable<Project> projects = (from project in _context.Projects select project)
                .Include(p => p.CreatedByNavigation)
                .Include(p => p.Users)
                .Select(p => new Project
                {
                    ProjectId = p.ProjectId,
                    Title = p.Title,
                    Description = p.Description,
                    CreatedBy = p.CreatedBy,
                    CreatedByNavigation = p.CreatedByNavigation,
                    CreatedDate = p.CreatedDate,
                    Users = isAuthenticated 
                        ? p.Users.Where(u => u.IsActive).ToList() // Om användaren är inloggad så visar alla aktiva användare
                        : p.Users.Where(u => !u.IsPrivate).Where(u => u.IsActive).ToList() // Om man inte är inloggad visas inte privata profiler
                });
            projectViewModel.Projects = projects;

            return View(projectViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Project project)
        {
            // hämtar id från inloggad användare och sätter det som skapare på projektet.
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            project.CreatedBy = userid;
            project.CreatedDate = DateTime.Now;
            _context.Add(project); //lägger till projektet i databasen
            _context.SaveChanges();
            return RedirectToAction("Index", "Project");
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            Project project = _context.Projects.Find(id);
            _context.Projects.Remove(project); //tar bort projektet från databasen
            _context.SaveChanges();
            return RedirectToAction("Index", "Project");

        }
        

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);
            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project updatedProject)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == updatedProject.ProjectId);
            if (!ModelState.IsValid)
            {
                return View(project); //visar vyn igen med valideringsfel
            }
            
            //uppdaterar projektet och sparar i databasen
            project.Title = updatedProject.Title;
            project.Description = updatedProject.Description;
            _context.SaveChanges();
            return RedirectToAction("Index", "Project");

        }

        [HttpPost]
        public IActionResult AddUserToProject(int projectId, string userId, string route)
        {
            //hämtar projekt och kopplade users
            var project = _context.Projects.Include(p => p.Users)
                                           .FirstOrDefault(p => p.ProjectId == projectId);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            
            if (project.Users.Any(u => u.Id == user.Id))
            {
                return BadRequest("Användaren är redan kopplad till projektet.");
            }
            project.Users.Add(user); //lägger till användaren till projektet
            _context.SaveChanges();
            return RedirectToAction("Index", route);
        }
    }
}
