using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cvprojekt.Models;

namespace cvprojekt.Controllers;

[Authorize]
public class MessagesController : Controller
{
    private readonly CvDbContext _context;

    public MessagesController(CvDbContext context)
    {
        _context = context;
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


        [HttpPost]
        public async Task<IActionResult> SendMessage(string reciever, string content)
        {
        
            var senderId = User.Identity.IsAuthenticated
                ? _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id
                : null;

            if (senderId == null)
                return Unauthorized("Användaren kunde inte hittas.");

            var message = new Message
            {
                Sender = senderId,
                Reciever = reciever,
                Content = content,
                TimeSent = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
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