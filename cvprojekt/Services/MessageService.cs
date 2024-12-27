using cvprojekt.Models;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Messages.CountAsync(m => m.Reciever == userId && m.IsRead == false);
        }
    }
}