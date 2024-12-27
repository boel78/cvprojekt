using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cvprojekt.Models;
using Microsoft.AspNetCore.Identity;

namespace cvprojekt.Controllers;

[Authorize]
public class MessagesController : Controller
{
    private readonly CvDbContext _context;
    private readonly UserManager<User> _userManager;

    public MessagesController(CvDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Visa mottagna meddelanden
    public async Task<IActionResult> Index()
    {
        
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var messages = await _context.Messages
            .Where(m => m.Reciever == userId)
            .Include(m => m.SenderNavigation)
            .OrderByDescending(m => m.TimeSent)
            .ToListAsync();

        return View(messages);
    }

    [HttpGet]
    public IActionResult SendMessage()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> SendMessage(string reciever, string content)
    {
        string userid = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userid);
        List<User> users = (from u in _context.Users
            where u != user
            select u).ToList();
        User recieverUser = users.FirstOrDefault(u => u.UserName == reciever);

        if (!users.Contains(recieverUser))
        {
            ModelState.AddModelError(string.Empty, "Det finns ingen användare med det namnet");
        }
        else
        {
            var senderId = User.Identity.IsAuthenticated
                ? _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id
                : null;

            if (senderId == null)
                return Unauthorized("Användaren kunde inte hittas.");
            
            var message = new Message
            {
                Sender = senderId,
                Reciever = recieverUser.Id,
                Content = content,
                TimeSent = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        return View();
    }
        
    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int mid)
    {
        var message = await _context.Messages.FindAsync(mid);
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (message == null || message.Reciever != userId)
        {
            return Unauthorized();
        }

        message.IsRead = true;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteMessage(int mid)
    {
        var message = await _context.Messages.FindAsync(mid);
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (message == null || message.Reciever != userId)
        {
            return Unauthorized();
        }

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
   
}