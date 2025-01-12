﻿using System.Diagnostics;
using cvprojekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

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

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _dbContext.Users
                .Include(u => u.Cvs)
                .ThenInclude(cv => cv.Educations)
                .ThenInclude(edu => edu.Skills)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var cvs = user.Cvs.ToList();
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

       
        private async Task AddEducation(EducationSkillViewModel viewmodel)
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
            List<Skill> skills = await _dbContext.Skills.ToListAsync();
            List<Skill> skillsToDb = skills.Where(s => !skillnames.Contains(s.Name)).ToList();

            foreach (var skill in skillsToDb)
            {
                        //Console.WriteLine("Skill " + skillinput);
                        await AddSkill(new Skill { Name = skill.Name });
                    
                
            }
            List<Skill> skillsToAdd = await _dbContext.Skills.Where(s => skillnames.Contains(s.Name)).ToListAsync();


            string userid = _userManager.GetUserId(User);

            var user = _dbContext.Users.Where(u => u.Id == userid).Include(u => u.Cvs).FirstOrDefault();
            education.Skills = skillsToAdd;
            education.Cvid = user.Cvs.FirstOrDefault().Cvid;
            _dbContext.Educations.Add(education);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddSkill(Skill skill)
        {
            List<Skill> skills = await _dbContext.Skills.ToListAsync();
            if (!skills.Contains(skill))
            {
                _dbContext.Skills.Add(skill);
            }
            
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
        [Authorize]
        public async Task<IActionResult> EditCV()
        {
            string userid = _userManager.GetUserId(User);
            int cvCount = _dbContext.Cvs.Where(c => c.Owner == userid).ToList().Count();
            
            
            Cv cv;
            var model = new CvViewModel();
            //Kollar efter om användaren har ett cv eller inte
            if (cvCount > 0)
            {
                cv = await _dbContext.Cvs.Where(c => c.Owner == userid)
                    .Include(c => c.Educations)
                    .ThenInclude(e => e.Skills)
                    .FirstOrDefaultAsync();
                
                
                
                model = new CvViewModel
                {
                    Cvid = cv.Cvid,
                    Description = cv.Description,
                    Educations = cv.Educations.Select(e => new EducationSkillViewModel
                    {
                        Eid = e.Eid,
                        Title = e.Title,
                        Description = e.Description,
                        Skills = string.Join(",", e.Skills.Select(s => s.Name))
                    }).ToList()
                };

                
                if (cv == null)
                {
                    return NotFound();
                }
            }
            //Om det inte finns cv så sätter vi cvid = 0 och tomma fält
            else if(cvCount == 0)
            {
                model = new CvViewModel
                {
                    Cvid = 0,
                    Description = "",
                    Educations = []
                };
            }

            ViewBag.options = new SelectList(_dbContext.Skills, "Name", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditCV(CvViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            
            
            if (ModelState.IsValid)
            {
                Cv cv;
                if (model.Cvid == 0)
                {
                    cv = new Cv
                    {
                        Description = model.Description,
                        Owner = userId
                    };
                    _dbContext.Cvs.Add(cv);
                }
                else
                {
                    cv = await _dbContext.Cvs.Where(c => c.Owner == userId)
                        .Include(c => c.Educations)
                        .ThenInclude(e => e.Skills)
                        .FirstOrDefaultAsync();

                    if (cv == null)
                    {
                        return NotFound();
                    }

                    cv.Description = model.Description;
                    _dbContext.Update(cv);
                }
                
                // Remove existing educations that are not in the model
                var educationsToRemove = cv.Educations.Where(e => !model.Educations.Any(em => em.Eid == e.Eid)).ToList();
                foreach (var education in educationsToRemove)
                {
                    //fixade constraints för att ta bort
                    education.Skills.Clear();
                    cv.Educations.Remove(education);
                    _dbContext.Educations.Remove(education);
                }

                foreach (var educationModel in model.Educations)
                {
                    await AddEducation(educationModel);
                    /*Education education = new Education()
                    {
                        Title = educationModel.Title,
                        Description = educationModel.Description,
                    };
                    if (education != null)
                    {
                        string[] skillnames = educationModel.Skills.Split(',');
                        List<Skill> skills = await _dbContext.Skills.ToListAsync();
                        List<Skill> skillsToDb = skills.Where(s => !skillnames.Contains(s.Name)).ToList();

                        Console.WriteLine("skills");
                        foreach (var skill in skillsToDb)
                        {
                            Console.WriteLine(skill.Name);
                            //await AddSkill(new Skill { Name = skill.Name });
                        }

                        List<Skill> skillsToAdd = await _dbContext.Skills.Where(s => skillnames.Contains(s.Name)).ToListAsync();


                        education.Title = educationModel.Title;
                        education.Description = educationModel.Description;
                        education.Skills = skillsToAdd;
                        _dbContext.Update(education);
                    }
                    else
                    {
                        education = new Education
                        {
                            Title = educationModel.Title,
                            Description = educationModel.Description,
                            Skills = educationModel.Skills.Split(',').Select(s => new Skill { Name = s }).ToList(),
                            Cvid = cv.Cvid
                        };
                        _dbContext.Educations.Add(education);
                    }*/

                }

                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.options = new SelectList(_dbContext.Skills, "Name", "Name");
            return View(model);
        }
    }
}
