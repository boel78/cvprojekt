using cvprojekt.Models;
using Microsoft.EntityFrameworkCore;
using Models;


namespace cvprojekt.Services
{
    public class MessageService
    {
        private readonly CvDbContext _context;

        public MessageService(CvDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetUnreadMessagesCountAsync(string userId)
        {
            // Räknar antalet meddelanden där mottagaren är userId och IsRead är false
            return await _context.Messages.CountAsync(m => m.Reciever == userId && m.IsRead == false);
        }
    }
}