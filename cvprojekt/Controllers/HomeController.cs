using System.Diagnostics;
using cvprojekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cvprojekt.Services;
using Microsoft.AspNetCore.Identity;
using Models;


namespace cvprojekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CvDbContext _context;
        private readonly MessageService _messageService;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, CvDbContext context, MessageService messageService,
            UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _messageService = messageService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel im = new IndexViewModel();

            //Fyller viewbag med "olästa" meddelanden
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount =
                userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            
            //Hämtar tre senaste projekten
            IQueryable<Project> projectList = (from Project in _context.Projects select Project)
                .OrderByDescending(p => p.CreatedDate).Include(p => p.Users).Take(1);
            
            im.projects = projectList;
            im.isActive = false;

            if (User.Identity.IsAuthenticated)
            {
                //hämta cvn som har aktiva skribenter
                IQueryable<Cv> cvList = (from Cv in _context.Cvs where Cv.OwnerNavigation.IsActive == true select Cv)
                    .Include(c => c.Educations)
                    .ThenInclude(e => e.Skills).Include(c => c.OwnerNavigation);

                //Fyll viewmodel med info, inklusive om det är en user is authenticated, annars är det false
                User user = await _userManager.FindByIdAsync(userId);
                im.cvs = cvList;
                im.isActive = user.IsActive;
            }
            else
            {
                //Hämtar icke privata och aktiva cvn
                IQueryable<Cv> cvList = (from Cv in _context.Cvs
                        where Cv.OwnerNavigation.IsActive == true
                        where Cv.OwnerNavigation.IsPrivate == false
                        select Cv)
                    .Include(c => c.Educations)
                    .ThenInclude(e => e.Skills).Include(c => c.OwnerNavigation);

                im.cvs = cvList;
            }
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            return View(im);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}