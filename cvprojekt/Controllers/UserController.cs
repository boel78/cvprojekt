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

namespace cvprojekt.Controllers
{
    public class UserController : Controller
    {
        private readonly CvDbContext _ctx;
        private readonly UserManager<User> _userManager;

        public UserController(CvDbContext ctx, UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _ctx.Users.Find(userid);

            var model = new EditUserViewModel
            {
                Name = user.Name,
                Email = user.Email,
                IsPrivate = user.IsPrivate,
                IsActive = user.IsActive,
            };


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel evm)
        {
            if (!ModelState.IsValid)
            {
                return View(evm);
            }

            var user = await _userManager.GetUserAsync(User);

            if (!string.IsNullOrEmpty(evm.NewPassword) && !string.IsNullOrEmpty(evm.CurrentPassword))
            {
                var result = await _userManager.ChangePasswordAsync(user, evm.CurrentPassword, evm.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(evm); // Returnera formuläret med felmeddelanden
                }
            }

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
        public IActionResult Search(string searchWord)
        {
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
                    //Söker på namn eller erfarenheter
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
                    byte[] p1 = null;
                    using (var fs1 = image.OpenReadStream())
                    using (var msa1 = new MemoryStream())
                    {
                        fs1.CopyTo(msa1);
                        p1 = msa1.ToArray();
                    }

                    user.ProfilePicture = p1;
                }
            }

            _ctx.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SerializeProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SerializeProfile(string username)
        {
            Console.WriteLine("user" + username);
            //path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.xml");
            //XmlSerializer xml = new XmlSerializer(typeof(XmlContainer));
            //FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);

            //XmlContainer container = new XmlContainer();

            //xml.Serialize(fileStream, container);
            //fileStream.Close();

            var user = await _userManager.Users
                .Include(u => u.Cvs)
                .ThenInclude(c => c.Educations).ThenInclude(e => e.Skills)
                .Include(u => u.ProjectsNavigation)
                .FirstOrDefaultAsync(u => u.UserName == username.Replace("/", ""));

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
            //Kan bytas ut om man vill ta en annan user
            var userId = (await _userManager.GetUserAsync(User)).Id;

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
            Console.WriteLine("skillS: " + skills.Count());
            foreach (var skill in skills)
            {
                Console.WriteLine(skill);
            }

            IQueryable<User> matches = _ctx.Users.Include(u => u.Cvs)
                .ThenInclude(c => c.Educations).ThenInclude(e => e.Skills).Where(u => u.Id != userId)
                .Where(u => u.Cvs.SelectMany(c => c.Educations).SelectMany(e => e.Skills)
                    .Any(skill => skills.Contains(skill.Name)));
            Console.WriteLine("Mathces: " + matches.Count());
            return View(matches);
        }
    }
}