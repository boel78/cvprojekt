using System.Linq;
using cvprojekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using cvprojekt.Services;
using Models;


namespace cvprojekt.Controllers
{
    public class ProjectController : Controller
    {
        private readonly CvDbContext _context;
        private readonly UserManager<User> _usermanager;
        private readonly MessageService _messageService;
        public ProjectController(CvDbContext context, UserManager<User> usermanager, MessageService messageService)
        {
            _context = context;
            _usermanager = usermanager;
            _messageService = messageService;
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
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            return View(projectViewModel);
        }

        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            // hämtar id från inloggad användare och sätter det som skapare på projektet.
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userid);
            project.CreatedBy = userid;
            project.CreatedDate = DateTime.Now;
            project.Users.Add(user);
            _context.Add(project);
            _context.SaveChanges();
            return RedirectToAction("Index", "Project");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            Project project = await _context.Projects.Include(p => p.Users).FirstOrDefaultAsync(p => p.ProjectId == id);
            
            foreach (var user in project.Users.ToList())
            {
                user.Projects.Remove(project); // Tar bort projektet från användarens lista
            }
            _context.Projects.Remove(project); //tar bort projektet från databasen
            _context.SaveChangesAsync();
            return RedirectToAction("Index", "Project");

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);
            var user = await _usermanager.GetUserAsync(User);
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
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
