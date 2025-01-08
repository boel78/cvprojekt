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

    // Visa mottagna meddelanden
    public async Task<IActionResult> Index()
    {

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var messages = await _context.Messages
            .Where(m => m.Reciever == userId)
            .Include(m => m.SenderNavigation)
            .OrderByDescending(m => m.TimeSent)
            .ToListAsync();
        ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
        return View(messages);
    }

    [HttpGet]
    public async Task<IActionResult> SendMessage(string username)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        ViewBag.UnreadMessagesCount = userId != null ? await _messageService.GetUnreadMessagesCountAsync(userId) : 0;
        if (username != null)
        {
            ViewData["Username"] = username.Replace("/", "");

        }
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> SendMessage(string reciever, string content, string sender)
    {
        string userid = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userid);
        List<User> users = (from u in _context.Users
            where u != user
            select u).ToList();
        User recieverUser = users.FirstOrDefault(u => u.UserName == reciever);

        //Kollar om mottagaren finns och om meddelandet är tomt 
        if (!users.Contains(recieverUser))
        {
            ModelState.AddModelError(string.Empty, "Det finns ingen användare med det användarnamnet");
            ViewData["MessageSent"] = false;
        } else if (content == null)
        {
            ModelState.AddModelError(string.Empty, "Meddelandet får inte vara tomt.");
            ViewData["MessageSent"] = false;
        }
        else
        {
            //Hämtar användarens id(den som skickat)
            var senderId = _userManager.GetUserId(User);
            var senderUser = _context.Users.FirstOrDefault(u => u.Name == sender);

            //Om användarnamnet inte är tomt
            if (sender != null)
            {
                if (users.Contains(senderUser))
                {
                    senderId = senderUser.Id;
                }
                else
                {
                    //Om avsändaren inte finns i databasen/icke inloggad användare skapas en temporär användare
                    //Den tas sedan bort när meddelandet tas bort. Identifieraren för det är Name="anonym"
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

            //Skapar meddelande objekt med avsändarId som vi satte ovanför
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
            ViewData["MessageSent"] = true;
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
        User sender = await _context.Users.Where(u => u.Id == message.Sender).FirstOrDefaultAsync();
        
        if (message == null || message.Reciever != userId)
        {
            return Unauthorized();
        }

        //Ta bort användaren om det är en anonym användare
        if (sender.Name == "anonym")
        {
            _context.Users.Remove(sender);
        }
        
        _context.Messages.Remove(message);
        
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

}
