﻿using cvprojekt.Models;
using cvprojekt.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Xml;
using System.Xml.Serialization;
using cvprojekt.Services;
using Models;


namespace cvprojekt.Controllers
{
    public class UserController : Controller
    {
        private readonly CvDbContext _ctx;
        private readonly UserManager<User> _userManager;
        private readonly MessageService _messageService;

        public UserController(CvDbContext ctx, UserManager<User> userManager, MessageService messageService)
        {
            _ctx = ctx;
            _userManager = userManager;
            _messageService = messageService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UnreadMessagesCount = userid != null ? await _messageService.GetUnreadMessagesCountAsync(userid) : 0;
            User user = _ctx.Users.Find(userid);

            var model = new EditUserViewModel
            {
                Name = user.Name,
                Email = user.Email,
                IsPrivate = user.IsPrivate,
                IsActive = user.IsActive,
            };

            EditUserChangePasswordViewModel em = new EditUserChangePasswordViewModel();
            em.editUserViewModel = model;
                
            return View(em);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserChangePasswordViewModel model)
        {
            ModelState.Remove("changePasswordViewModel.CurrentPassword");
            ModelState.Remove("changePasswordViewModel.NewPassword");
            ModelState.Remove("changePasswordViewModel.ConfirmNewPassword");
            EditUserViewModel evm = model.editUserViewModel;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            

            // Uppdatera användarens andra uppgifter
            user.Name = evm.Name;
            user.Email = evm.Email;
            user.IsPrivate = evm.IsPrivate;
            user.IsActive = evm.IsActive;

            await _ctx.SaveChangesAsync();

            TempData["SuccessMessage"] = "Dina ändringar har sparats.";
            return RedirectToAction("Edit", "User");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchWord)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            List<User> users = new List<User>();

            if (User.Identity.IsAuthenticated)
            {
                //Om Användaren är inloggad så visas alla aktiva användare(Inget sökord)
                users = (from user in _ctx.Users where user.IsActive == true select user).Include(u => u.Cvs)
                    .ThenInclude(c => c.Educations)
                    .ThenInclude(e => e.Skills).ToList();

                //Om man har skrivit i något sökord
                if (!searchWord.IsNullOrEmpty())
                {
                    //Söker på namn eller erfarenheter delar sökorden på mellanslag
                    string[] searchWords = searchWord.Split(' ');
                    users = users.Where(u => searchWords.Any(word =>
                        u.Name.Trim().ToLower().Contains(word.Trim().ToLower())
                        || u.Cvs.Any(cv => cv.Educations
                            .Any(edu => edu.Skills
                                .Any(sid => searchWords.Any(word =>
                                    sid.Name.Trim().ToLower().Contains(word.Trim().ToLower()))))))).ToList();
                }
            }
            else
            {
                //Gör som koden ovanför fast ser till att privata users inte visas
                users = (from user in _ctx.Users where user.IsPrivate == false where user.IsActive == true select user)
                    .Include(u => u.Cvs)
                    .ThenInclude(c => c.Educations)
                    .ThenInclude(e => e.Skills)
                    .ToList();

                if (!searchWord.IsNullOrEmpty())
                {
                    string[] searchWords = searchWord.Split(' ');
                    users = users.Where(u => searchWords.Any(word =>
                        u.Name.Trim().ToLower().Contains(word.Trim().ToLower())
                        || u.Cvs.Any(cv => cv.Educations
                            .Any(edu => edu.Skills
                                .Any(sid => searchWords.Any(word =>
                                    sid.Name.Trim().ToLower().Contains(word.Trim().ToLower()))))))).ToList();
                }
            }

            return View(users);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Image()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            User user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Image(IFormFile image)
        {
            User user = await _userManager.GetUserAsync(User);
            if (image != null)
            {
                if (image.Length > 0)
                {
                    //Gör om filen till bytes
                    byte[] p1 = null;
                    using (var fs1 = image.OpenReadStream())
                    using (var msa1 = new MemoryStream())
                    {
                        fs1.CopyTo(msa1);
                        p1 = msa1.ToArray();
                    }
                    //Uppdaterar profilen
                    user.ProfilePicture = p1;
                }
            }

            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> SerializeProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SerializeProfile(string username)
        {
            //Inkluderar fälten som behövs i xmlen
            var user = await _userManager.Users
                .Include(u => u.Cvs)
                .ThenInclude(c => c.Educations).ThenInclude(e => e.Skills)
                .Include(u => u.ProjectsNavigation)
                .FirstOrDefaultAsync(u => u.UserName == username.Replace("/", ""));

            //Fyller Dto som används för att formatera xml filen
            var userDto = new UserDto
            {
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email,
                Cvs = user.Cvs.Select(cv => new CvDto
                {
                    Description = cv.Description,
                    Educations = cv.Educations.Select(e => new EducationDto
                    {
                        Title = e.Title,
                        Description = e.Description,
                        Skills = e.Skills.Select(s => new SkillDto
                        {
                            Name = s.Name
                        }).ToList()
                    }).ToList()
                }).ToList(),
                Projects = user.ProjectsNavigation.Select(p => new ProjectDto
                {
                    Title = p.Title,
                    Description = p.Description,
                }).ToList()
            };

            //Startar serialisering
            var xmlSerializer = new XmlSerializer(typeof(UserDto));

            using (var stream = new MemoryStream())
            {
                xmlSerializer.Serialize(stream, userDto);
                stream.Position = 0;


                return File(stream.ToArray(), "application/xml", "user_info.xml");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Match()
        {
            //Matchar från den inloggades kompetenser
            var userId = (await _userManager.GetUserAsync(User)).Id;
            ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;

            //hämtar all info som behövs till matchningen
            //Blir en del kod eftersom Cv -> Erfarenhet -> Kompetens
            var user = await _ctx.Users
                .Include(u => u.Cvs)
                .ThenInclude(cv => cv.Educations)
                .ThenInclude(edu => edu.Skills)
                .FirstOrDefaultAsync(u => u.Id == userId);

            List<string> skills = user.Cvs
                .SelectMany(cv => cv.Educations)
                .SelectMany(edu => edu.Skills)
                .Select(skill => skill.Name)
                .ToList();

            //Hämtar användare som har liknande kompetenser
            IQueryable<User> matches = _ctx.Users.Include(u => u.Cvs)
                .ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).Where(u => u.Id != userId)
                .Where(u => u.Cvs.SelectMany(c => c.Educations).SelectMany(e => e.Skills)
                    .Any(skill => skills.Contains(skill.Name)));
            return View(matches);
        }
    }
}