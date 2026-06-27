using Microsoft.EntityFrameworkCore;
using MondakiComics.Data;
using MondakiComics.Repositories.Interfaces;

namespace MondakiComics.Repositories
{
    public class ContactMessageRepository : BaseRepository<ContactMessage>, IContactMessageRepository
    {
        public ContactMessageRepository(MondakiDbContext context) : base(context) { }

        public async Task<IEnumerable<ContactMessage>> GetAllAsync()
        {
            return await context.ContactMessages
                .OrderByDescending(m => m.ReceivedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await context.ContactMessages
                .CountAsync(m => !m.IsRead);
        }
    }
}