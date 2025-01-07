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
            List<SelectListItem> skills = _dbContext.Skills.Select(x => new SelectListItem { Text = x.Name, Value = x.Name }).ToList();
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
            await _dbContext.SaveChangesAsync();
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
                    vm.Projects = await _dbContext.Projects.Where(p => p.Users.Contains(vm.User)).Include(p => p.CreatedByNavigation).Include(p => p.Users).ToListAsync();
                }
                
                //Plussar på 1 varje gång sidan laddas, om det inte är en själv
                if (vm.User.Cvs.Count > 0)
                {
                    if (vm.User.Id != _userManager.GetUserId(User))
                    {
                        Cv cv = vm.User.Cvs.FirstOrDefault();
                        CvView cvv = _dbContext.CvViews.Where(cvv => cvv.Cvid == cv.Cvid).FirstOrDefault();
                        cvv.ViewCount = cvv.ViewCount + 1;
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        Cv cv = vm.User.Cvs.FirstOrDefault();
                        vm.IsWriter = true;
                        vm.ViewCount = _dbContext.CvViews.Where(cvv => cvv.Cvid == cv.Cvid).Select(cvv => cvv.ViewCount).FirstOrDefault();
                    }
                    
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

                if (User.Identity.IsAuthenticated)
                {
                    //Om användaren är inloggad visas matchingar på dom som inte är privata
                    vm.UsersMatch = _dbContext.Users.Where(u => u.IsActive == true)                
                        .Include(u => u.Cvs)
                        .ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).Where(u => u.Id != userId)
                        .Where(u => u.Cvs.SelectMany(c => c.Educations).SelectMany(e => e.Skills)
                            .Any(skill => skills.Contains(skill.Name)));
                }
                else
                {
                    //Om användaren inte är inloggad visas matchningar på dom som inte är privata
                    vm.UsersMatch = _dbContext.Users.Where(u => u.IsPrivate == false).Where(u => u.IsActive == true)
                        .Include(u => u.Cvs)
                        .ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).Where(u => u.Id != userId)
                        .Where(u => u.Cvs.SelectMany(c => c.Educations).SelectMany(e => e.Skills)
                            .Any(skill => skills.Contains(skill.Name)));
                }
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> EditCV()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _dbContext.Users
                            .Include(u => u.Cvs)
                            .ThenInclude(cv => cv.Educations)
                            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Cvs.Count == 0)
            {
                return NotFound();
            }

            var cv = user.Cvs.FirstOrDefault();
            var education = cv.Educations.FirstOrDefault();
            var model = new EducationSkillViewModel
            {
                Title = education?.Title,
                Description = education?.Description,
                Skills = education != null ? string.Join(",", education.Skills.Select(s => s.Name)) : string.Empty
            };

            ViewBag.options = new SelectList(_dbContext.Skills, "Name", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCV(EducationSkillViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var user = await _dbContext.Users
                    .Include(u => u.Cvs)
                    .ThenInclude(cv => cv.Educations)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.Cvs.Count == 0)
                {
                    return NotFound();
                }

                var cv = user.Cvs.FirstOrDefault();
                var education = cv.Educations.FirstOrDefault();

                if (education != null)
                {
                    education.Title = model.Title;
                    education.Description = model.Description;
                    education.Skills = model.Skills.Split(',').Select(s => new Skill { Name = s }).ToList();
                    _dbContext.Update(education);
                }
                else
                {
                    education = new Education
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Skills = model.Skills.Split(',').Select(s => new Skill { Name = s }).ToList(),
                        Cvid = cv.Cvid
                    };
                    _dbContext.Add(education);
                }

                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.options = new SelectList(_dbContext.Skills, "Name", "Name");
            return View(model);
        }
    }
}