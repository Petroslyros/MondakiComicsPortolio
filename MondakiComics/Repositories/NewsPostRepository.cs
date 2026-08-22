using Microsoft.EntityFrameworkCore;
using MondakiComics.Data;
using MondakiComics.Repositories.Interfaces;

namespace MondakiComics.Repositories
{
    public class NewsPostRepository : BaseRepository<NewsPost>, INewsPostRepository
    {
        public NewsPostRepository(MondakiDbContext context) : base(context) { }

        public async Task<IEnumerable<NewsPost>> GetPublishedAsync()
        {
            return await context.NewsPosts
                .Where(n => n.IsPublished && !n.IsDeleted)
                .OrderByDescending(n => n.InsertedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsPost>> GetAllForAdminAsync()
        {
            return await context.NewsPosts
                .Where(n => !n.IsDeleted)
                .OrderByDescending(n => n.InsertedAt)
                .ToListAsync();
        }
    }
}