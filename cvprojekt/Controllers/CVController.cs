using System.Diagnostics;
using cvprojekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using cvprojekt.Services;
using Microsoft.VisualBasic;
using Models;

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


        public async Task<IActionResult> Index()
        {
            List<Cv> allCv = await _dbContext.Cvs.Include(c => c.Educations)
                .ThenInclude(e => e.Skills)
                .Include(c => c.OwnerNavigation)
                .Where(c => c.OwnerNavigation.IsActive == true)
                .ToListAsync();
            if (!User.Identity.IsAuthenticated)
            {
                allCv = await _dbContext.Cvs.Include(c => c.Educations)
                    .ThenInclude(e => e.Skills)
                    .Include(c => c.OwnerNavigation)
                    .Where(c => c.OwnerNavigation.IsActive == true).Where(c => c.OwnerNavigation.IsPrivate == false)
                    .ToListAsync();
            }
            var userid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userid != null ? await _messageService.GetUnreadMessagesCountAsync(userid) : 0;
            return View(allCv);
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
            ViewBag.UnreadMessagesCount = userid != null ? await _messageService.GetUnreadMessagesCountAsync(userid) : 0;
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
                //Har användaren 0 cvn så skapa det
                if (model.Cvid == 0)
                {
                    cv = new Cv
                    {
                        Description = model.Description,
                        Owner = userId
                    };
                    _dbContext.Cvs.Add(cv);
                    _dbContext.SaveChanges();
                    var cvv = new CvView
                    {
                        Cvid = cv.Cvid,
                        ViewCount = 0,
                        Cv = cv
                    };
                    _dbContext.CvViews.Add(cvv);
                }
                else
                {
                    //Hämta cvt och uppdatera det
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
                
                // Tar bort educations som inte finns i model
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
                    //Om det inte redan finns en education i databasen som i model så lägg till
                    if (!cv.Educations.Any(e => e.Eid == educationModel.Eid))
                    {
                        await AddEducation(educationModel);
                    }
                    
                    //Kontrollerar om en education ska uppdateras
                    if (_dbContext.Educations.Any(e => e.Eid == educationModel.Eid))
                    {
                        await UpdateEducation(educationModel, cv);
                    }
                }

                await _dbContext.SaveChangesAsync();
                return RedirectToAction("ShowCv");
            }

            ViewBag.options = new SelectList(_dbContext.Skills, "Name", "Name");
            return View(model);
        }

        private async Task AddEducation(EducationSkillViewModel viewmodel)
        {
            string revampedstring = viewmodel.Skills.Replace("\"", "").Replace("[", "").Replace("]", "");
            string[] skillnames = revampedstring.Split(',');

            Education education = new Education()
            {
                Title = viewmodel.Title,
                Description = viewmodel.Description,
            };
            List<Skill> skills = await _dbContext.Skills.ToListAsync();
            //nya skills som behöver läggas till i databasen
            var skillsToDb = skillnames
                .Select(skillname => skillname.TrimStart(' ').ToLower())
                .Where(skillName => !skills.Any(existingSkill => existingSkill.Name.ToLower() == skillName))
                .Distinct()
                .ToList();
            List<Skill> newSkills = new List<Skill>();
            foreach (var skill in skillsToDb)
            {
                var newSkill = new Skill { Name = skill };
                await AddSkill(newSkill);
                newSkills.Add(newSkill);
            }
            //Skills som ska läggas till i respektive education
            List<Skill> skillsToAdd = await _dbContext.Skills.Where(s => skillnames.Contains(s.Name) || newSkills.Select(ns => ns.Name).Contains(s.Name)).ToListAsync();


            string userid = _userManager.GetUserId(User);

            var user = _dbContext.Users.Where(u => u.Id == userid).Include(u => u.Cvs).FirstOrDefault();
            education.Skills = skillsToAdd;
            education.Cvid = user.Cvs.FirstOrDefault().Cvid;
            _dbContext.Educations.Add(education);
            await _dbContext.SaveChangesAsync();
        }

        private async Task AddSkill(Skill skill)
        {
            List<Skill> skills = await _dbContext.Skills.ToListAsync();
            if (!skills.Contains(skill))
            {
                _dbContext.Skills.Add(skill);
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task UpdateEducation(EducationSkillViewModel educationModel, Cv cv)
        {
            int eid = educationModel.Eid;
            var education = cv.Educations.Where(e => e.Eid == eid).FirstOrDefault();
            education.Skills.Clear();
            cv.Educations.Remove(education);
                        
            _dbContext.Educations.Remove(education);
            await _dbContext.SaveChangesAsync();
            await AddEducation(educationModel);
        }

        //Tar username eftersom det är en get och är känsligt med id
        [HttpGet]
        public async Task<IActionResult> ShowCv(string username)
        {

            ShowCvViewModel vm = new ShowCvViewModel();

            //Väljer rätt user
            List<User> users = await _dbContext.Users.ToListAsync();
                //Hämtar usern och projects som den deltar i. Inkluderar de models som är viktiga. Fyller Viewmodel.
                if (User.Identity.IsAuthenticated && username == null)
                {
                    string id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    vm.User = _dbContext.Users.Where(u => u.Id == id).Include(u => u.Cvs).ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).FirstOrDefault();
                    vm.IsWriter = true;
                    
                }

                if (username != null)
                {
                    string id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        var loggedInUser = _dbContext.Users.Where(u => u.Id == id).FirstOrDefault();
                        vm.User = await _dbContext.Users.Where(u => u.UserName == username).Include(u => u.Cvs).ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).FirstOrDefaultAsync();
                        if (username == loggedInUser.UserName)
                        {
                            vm.IsWriter = true;
                        }

                }
                vm.Projects = await _dbContext.Projects.Where(p => p.Users.Contains(vm.User)).Include(p => p.CreatedByNavigation).Include(p => p.Users).ToListAsync();


                if (vm.User != null)
                {
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
                }

            var userIdForMessage = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userIdForMessage != null ? await _messageService.GetUnreadMessagesCountAsync(userIdForMessage) : 0;
        return View(vm);
        }
    }
}
