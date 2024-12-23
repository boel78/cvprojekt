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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);
            var user = await _usermanager.GetUserAsync(User);
            if (project.CreatedBy != user.Id) 
            {
                return Forbid();
            }
            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project updatedProject) 
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == updatedProject.ProjectId);
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            project.Title = updatedProject.Title;
            project.Description = updatedProject.Description;
            _context.SaveChanges();
            return RedirectToAction("Index", "Project");

        }

        [HttpPost]
        public IActionResult AddUserToProject(int projectId, string userId)
        {
            var project = _context.Projects.Find(projectId);
            var user = _context.Users.Find(userId);
            if (_context.UserProjects.AsQueryable().Any(up => up.UserID == userId && up.ProjectID == projectId))
            {
                return BadRequest("Användaren är redan kopplad till projektet.");
            }

            var userProject = new UserProject
            {
                UserID = userId,
                ProjectID = projectId
            };
            _context.UserProjects.Add(userProject);
            _context.SaveChanges();
            return RedirectToAction("Index", "Project");
        }
    }
}
