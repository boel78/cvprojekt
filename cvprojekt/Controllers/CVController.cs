﻿using System.Diagnostics;
using cvprojekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using cvprojekt.Services;

namespace cvprojekt.Controllers
{
    public class CVController : Controller
    {
        private readonly CvDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly MessageService _messageService;

        public CVController(CvDbContext dbContext, UserManager<User> userManager, MessageService messageService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _messageService = messageService;
        }


        public async Task<IActionResult> Index(string projekt)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            List<Cv> cvs = _dbContext.Cvs.Where(c => c.Projects.Any(p => p.Title.Contains(projekt))).ToList();

            return View(cvs);
        }

        [HttpGet]
        public async Task<IActionResult> AddEducation()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            List<SelectListItem> skills = _dbContext.Skills.Select(x => new SelectListItem { Text = x.Name, Value = x.Name}).ToList();
            SelectListItem newSkill = new SelectListItem { Text = "Lägg till en ny kompetens..", Value = "NewSkill" };
            skills.Insert(skills.Count, newSkill);
            ViewBag.options = skills;
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> AddEducation(EducationSkillViewModel viewmodel)
        {
            //Hämtar strängen från vyn som ser ut som "["erfarenhet1", "erfarenhet2"]" och gör om den till en array
            string revampedstring = viewmodel.Skills.Replace("\"", "").Replace("[", "").Replace("]", "");
            string[] skillnames = revampedstring.Split(',');
            
            //Skapar en ny erfarenhet
            Education education = new Education()
            {
                Title = viewmodel.Title,
                Description = viewmodel.Description,
            };
            
            //Kontrollerar ifall en kompetens inte finns, då skapas det en ny i databasen
            List<Skill> skills = await _dbContext.Skills.ToListAsync();
                foreach (var skillInput in skillnames)
                {
                    if (!skills.Any(s => s.Name.Equals(skillInput, StringComparison.OrdinalIgnoreCase)))
                    {
                        await AddSkill(new Skill { Name = skillInput });
                    }
                
                
            }
            //Gör en lista med kompetenser som ska kopplas till erfarenheten som skapas
            List<Skill> skillsToAdd = await _dbContext.Skills.Where(s => skillnames.Contains(s.Name)).ToListAsync();
            
            //Hämtar användaren och skapar erfarenheten och kopplar den till cvt
            var userid = _userManager.GetUserId(User);
            var user = await _dbContext.Users.Where(u => u.Id == userid).Include(u => u.Cvs).FirstOrDefaultAsync();
            education.Skills = skillsToAdd;
            education.Cvid = user.Cvs.FirstOrDefault().Cvid;
            _dbContext.Educations.Add(education);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        
        private async Task AddSkill(Skill skill)
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
                //Hämtar usern och projects som den deltar i. Inkluderar de models som är viktiga. Fyller Viewmodel.
                if (User.Identity.IsAuthenticated)
                {
                    string id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    vm.User = await _userManager.FindByIdAsync(id);
                }
                else
                {
                    vm.User = await _dbContext.Users.Where(u => u.UserName == username).Include(u => u.Cvs).ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).FirstOrDefaultAsync();
                }
                    vm.Projects = await _dbContext.Projects.Where(p => p.Users.Contains(vm.User)).Include(p => p.CreatedByNavigation).Include(p => p.Users).ToListAsync();
                
                
                
                //Plussar på 1 på tittarsiffror varje gång sidan laddas, om det inte är en själv
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
                var userId = vm.User.Id;

                //hämtar användarens erfarenheter
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
                    //Om användaren är inloggad visas matchingar efter kompetenser på dom som inte är privata
                    //Inkluderingar krävs eftersom våran databas är Cv -> Erfarenheter -> kompetenser
                    vm.UsersMatch = _dbContext.Users.Where(u => u.IsActive == true)                
                        .Include(u => u.Cvs)
                        .ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).Where(u => u.Id != userId)
                        .Where(u => u.Cvs.SelectMany(c => c.Educations).SelectMany(e => e.Skills)
                            .Any(skill => skills.Contains(skill.Name)));
                }
                else
                {
                    //Om användaren inte är inloggad visas matchningar efter kompetenser på dom som inte är privata
                    vm.UsersMatch = _dbContext.Users.Where(u => u.IsPrivate == false).Where(u => u.IsActive == true)
                        .Include(u => u.Cvs)
                        .ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).Where(u => u.Id != userId)
                        .Where(u => u.Cvs.SelectMany(c => c.Educations).SelectMany(e => e.Skills)
                            .Any(skill => skills.Contains(skill.Name)));
                }
                
            var userIdForMessage = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userIdForMessage != null ? await _messageService.GetUnreadMessagesCountAsync(userIdForMessage) : 0;
        return View(vm);
        }
    }
}
