using System.Diagnostics;
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
            List<SelectListItem> skills = _dbContext.Skills.Select(x => new SelectListItem { Text = x.Name, Value = x.Name}).ToList();
            SelectListItem newSkill = new SelectListItem { Text = "Lägg till en ny kompetens..", Value = "NewSkill" };
            skills.Insert(skills.Count, newSkill);
            ViewBag.options = skills;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEducation(EducationSkillViewModel viewmodel)
        {
            string revampedstring = viewmodel.Skills.Replace("\"", "").Replace("[", "").Replace("]", "");
            string[] skillnames = revampedstring.Split(',');
            
            
            foreach (var skill in skillnames)
            {
                Console.WriteLine("Skill " + skill);
            }
            Education education = new Education()
            {
                Title = viewmodel.Title,
                Description = viewmodel.Description,
            };
            IQueryable<Skill> skills = _dbContext.Skills;
            foreach (var skill in skills)
            {
                foreach (var skillinput in skillnames)
                {
                    if (!skill.Name.Equals(skillinput))
                    {
                        await AddSkill(new Skill { Name = skillinput });
                    }
                }
                
            }
            IQueryable<Skill> skillsToAdd = from s in skills
                where skillnames.Contains(s.Name)
                select s;
            
            string userid = _userManager.GetUserId(User);
    
            var user = _dbContext.Users.Where(u => u.Id == userid).Include(u => u.Cvs).FirstOrDefault();
            education.Skills = skillsToAdd.ToList();
            education.Cvid = user.Cvs.FirstOrDefault().Cvid;
            _dbContext.Educations.Add(education);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        
        public async Task AddSkill(Skill skill)
        {
            _dbContext.Skills.Add(skill);
            _dbContext.SaveChangesAsync();
        }

        //Tar username eftersom det är en get och är känsligt med id
        [HttpGet]
        public async Task<IActionResult> ShowCv(string username)
        {
            ShowCvViewModel vm = new ShowCvViewModel();
            
            //Väljer rätt user
            List<User> users = await _dbContext.Users.ToListAsync();
            vm.User = new User();
            if (!string.IsNullOrEmpty(username))
            {
                foreach (var theuser in users)
                {
                    vm.User = await _dbContext.Users.Where(u => u.UserName == username).Include(u => u.Cvs).ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).FirstOrDefaultAsync();
                }
                
                //Plussar på 1 varje gång sidan laddas
                if (vm.User.Cvs.Count > 0)
                {
                    Cv cv = vm.User.Cvs.FirstOrDefault();
                    CvView cvv = _dbContext.CvViews.Where(cvv => cvv.Cvid == cv.Cvid).FirstOrDefault();
                    cvv.ViewCount = cvv.ViewCount + 1;
                    _dbContext.SaveChanges();
                }
                
                
                
                //Hämtar matchningar
                //Kan bytas ut om man vill ta en annan user
                var userId = vm.User.Id;

                var user = await _dbContext.Users
                    .Include(u => u.Cvs)
                    .ThenInclude(cv => cv.Educations)
                    .ThenInclude(edu => edu.Skills)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            
                List<string> skills = user.Cvs
                    .SelectMany(cv => cv.Educations)
                    .SelectMany(edu => edu.Skills)
                    .Select(skill => skill.Name)
                    .ToList();
                Console.WriteLine("skillS: " + skills.Count());
                foreach (var skill in skills)
                {
                    Console.WriteLine(skill);
                }

                vm.UsersMatch = _dbContext.Users.Include(u => u.Cvs)
                    .ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).Where(u => u.Id != userId)
                    .Where(u => u.Cvs.SelectMany(c => c.Educations).SelectMany(e => e.Skills).Any(skill => skills.Contains(skill.Name)));
                
                
            }
 
        return View(vm);
        }
    }
}
