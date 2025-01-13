using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cvprojekt.Models;
using cvprojekt.Services;
using Microsoft.AspNetCore.Identity;
using Models;

namespace cvprojekt.Controllers;

public class MessagesController : Controller
{
    private readonly CvDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly MessageService _messageService;
    
    public MessagesController(CvDbContext context, UserManager<User> userManager, MessageService messageService)
    {
        _context = context;
        _userManager = userManager;
        _messageService = messageService;
    }

    // Visar mottagna meddelanden för den inloggade användaren
    public async Task<IActionResult> Index()
    {
        // Hämta den inloggade användarens ID
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Hämta meddelanden där den inloggade användaren är mottagare
        var messages = await _context.Messages
            .Where(m => m.Reciever == userId)
            .Include(m => m.SenderNavigation) // Inkludera avsändarens information
            .OrderByDescending(m => m.TimeSent) // Sortera meddelanden efter skickat tidpunkt (nyaste först)
            .ToListAsync();

        // Sätt antal olästa meddelanden i ViewBag för visning i vyn
        ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;

        return View(messages);
    }

    // GET-metod för att visa formulär för att skicka meddelanden
    [HttpGet]
    public async Task<IActionResult> SendMessage(string username)
    {
        // Hämta den inloggade användarens ID
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Sätt antal olästa meddelanden i ViewBag
        ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;

        // Om ett användarnamn skickades in, skicka det till vyn via ViewData
        if (username != null)
        {
            ViewData["Username"] = username.Replace("/", "");
        }

        return View();
    }

    // POST-metod för att skicka ett meddelande
    [HttpPost]
    public async Task<IActionResult> SendMessage(string reciever, string content, string sender)
    {
        // Hämta den inloggade användaren
        string userid = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userid);

        // Hämta alla användare för validering
        List<User> users = (from u in _context.Users
                            where u != user
                            select u).ToList();

        // Kontrollera om mottagaren finns
        User recieverUser = users.FirstOrDefault(u => u.UserName == reciever);

        if (!users.Contains(recieverUser))
        {
            // Mottagaren finns inte
            ModelState.AddModelError(string.Empty, "Det finns ingen användare med det användarnamnet");
            ViewData["MessageSent"] = false;
        }
        else if (string.IsNullOrEmpty(content))
        {
            // Kontrollera att meddelandet inte är tomt
            ModelState.AddModelError(string.Empty, "Meddelandet får inte vara tomt.");
            ViewData["MessageSent"] = false;
        }
        else
        {
            // Hämta eller skapa avsändaren
            var senderId = _userManager.GetUserId(User);
            var senderUser = _context.Users.FirstOrDefault(u => u.Name == sender);

            if (sender != null)
            {
                if (users.Contains(senderUser))
                {
                    senderId = senderUser.Id; // Använd existerande avsändare
                }
                else
                {
                    // Skapa en temporär användare om avsändaren inte finns i databasen
                    User newUser = new User
                    {
                        UserName = sender,
                        Name = "anonym",
                        ProfilePicture = []
                    };
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    senderId = newUser.Id;
                }
            }

            // Skapa ett nytt meddelande
            var message = new Message
            {
                Sender = senderId,
                Reciever = recieverUser.Id,
                Content = content,
                TimeSent = DateOnly.FromDateTime(DateTime.Now), // Tidpunkt för skickandet
                IsRead = false // Markeras som oläst
            };

            _context.Messages.Add(message); // Lägg till i databasen
            await _context.SaveChangesAsync(); // Spara ändringar
            ViewData["MessageSent"] = true; // Indikera att meddelandet skickades
        }

        return View();
    }

    // POST-metod för att markera ett meddelande som läst
    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int mid)
    {
        // Hämta meddelandet baserat på ID
        var message = await _context.Messages.FindAsync(mid);

        // Kontrollera att meddelandet existerar och att användaren är mottagare
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (message == null || message.Reciever != userId)
        {
            return Unauthorized(); // Nekad åtkomst
        }

        // Markera meddelandet som läst
        message.IsRead = true;
        await _context.SaveChangesAsync(); // Spara ändringar

        return RedirectToAction("Index");
    }

    // POST-metod för att ta bort ett meddelande
    [HttpPost]
    public async Task<IActionResult> DeleteMessage(int mid)
    {
        // Hämta meddelandet baserat på ID
        var message = await _context.Messages.FindAsync(mid);

        // Hämta den inloggade användarens ID
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Kontrollera att meddelandet existerar och att användaren är mottagare
        if (message == null || message.Reciever != userId)
        {
            return Unauthorized(); // Nekad åtkomst
        }

        // Kontrollera om avsändaren är en temporär användare och ta bort dem
        User sender = await _context.Users.Where(u => u.Id == message.Sender).FirstOrDefaultAsync();
        if (sender != null && sender.Name == "anonym")
        {
            _context.Users.Remove(sender);
        }

        // Ta bort meddelandet
        _context.Messages.Remove(message);
        await _context.SaveChangesAsync(); // Spara ändringar

        return RedirectToAction("Index");
    }
}
