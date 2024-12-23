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
    }
}
