using Microsoft.EntityFrameworkCore;
using MondakiComics.Data;
using MondakiComics.Repositories.Interfaces;

namespace MondakiComics.Repositories
{

    public class ArtworkCategoryRepository :BaseRepository<ArtworkCategory>, IArtworkCategoryRepository
    {
        public ArtworkCategoryRepository(MondakiDbContext context) : base(context)
        {
        }

        public async Task<ArtworkCategory?> GetBySlugAsync(string slug)
        {
            return await context.ArtworkCategories
                .Where(c => c.Slug == slug)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await context.ArtworkCategories.AnyAsync(c => c.Slug == slug);
        }
    }
}
