using System.Diagnostics;

using cvprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using cvprojekt.Services;

namespace cvprojekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CvDbContext _context;
        private readonly MessageService _messageService;

        public HomeController(ILogger<HomeController> logger, CvDbContext context, MessageService messageService)
        {
            _logger = logger;
            _context = context;
            _messageService = messageService;
        }

        public async Task<IActionResult> Index()
        {
            

            //Kod f�r att d�lja icke aktiva
            //IQueryable<User> userList = from user in _context.Users select user;
            //if (showOnlyActive)
            //{
            //    userList = userList.Where(x => x.IsActive);
            //}
            //else
            //{
            //    userList = userList.Where(x => !x.IsActive);
            //}


            //ViewData["ShowOnlyActive"] = showOnlyActive;


            IndexViewModel im = new IndexViewModel();
            
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            if (User.Identity.IsAuthenticated)
            {
                IQueryable<Cv> cvList = (from Cv in _context.Cvs where Cv.OwnerNavigation.IsActive == true select Cv).Include(c => c.Educations)
                    .ThenInclude(e => e.Skills).Include(c => c.OwnerNavigation);

                IQueryable<Project> projectList = (from Project in _context.Projects select Project)
                    .OrderBy(p => p.CreatedDate).Take(3);

                im.projects = projectList;
                im.cvs = cvList;
            }
            else
            {
                IQueryable<Cv> cvList = (from Cv in _context.Cvs where Cv.OwnerNavigation.IsActive == true 
                                                where Cv.OwnerNavigation.IsPrivate == false select Cv)
                                                    .Include(c => c.Educations)
                                                        .ThenInclude(e => e.Skills).Include(c => c.OwnerNavigation);

                IQueryable<Project> projectList = (from Project in _context.Projects select Project)
                    .OrderByDescending(p => p.CreatedDate).Take(3);

                im.projects = projectList;
                im.cvs = cvList;
            }
            
            return View(im);
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