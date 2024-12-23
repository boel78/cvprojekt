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
                    .OrderBy(p => p.CreatedDate).Take(3);

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
