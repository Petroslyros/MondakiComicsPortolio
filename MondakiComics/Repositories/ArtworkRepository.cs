using Microsoft.EntityFrameworkCore;
using MondakiComics.Data;
using MondakiComics.Repositories.Interfaces;

namespace MondakiComics.Repositories
{
    public class ArtworkRepository : BaseRepository<Artwork>, IArtworkRepository
    {
        public ArtworkRepository(MondakiDbContext context) : base(context) { }

        public async Task<IEnumerable<Artwork>> GetPublishedAsync()
        {
            return await context.Artworks
                .Where(a => a.IsPublished && !a.IsDeleted)
                .Include(a => a.Category)
                .Include(a => a.Images)
                .OrderBy(a => a.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Artwork>> GetByCategoryAsync(int categoryId)
        {
            return await context.Artworks
                .Where(a => a.CategoryId == categoryId && a.IsPublished && !a.IsDeleted)
                .Include(a => a.Category)
                .Include(a => a.Images)
                .OrderBy(a => a.SortOrder)
                .ToListAsync();
        }

        public async Task<Artwork?> GetWithImagesAsync(int id)
        {
            return await context.Artworks
                .Where(a => a.Id == id && !a.IsDeleted)
                .Include(a => a.Category)
                .Include(a => a.Images.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Artwork>> GetAllForAdminAsync()
        {
            return await context.Artworks
                .Where(a => !a.IsDeleted)
                .Include(a => a.Category)
                .Include(a => a.Images)
                .OrderBy(a => a.SortOrder)
                .ToListAsync();
        }

        public async Task<bool> UpdateCoverImageAsync(int artworkId, string imageUrl)
        {
            var artwork = await context.Artworks.FindAsync(artworkId);
            if (artwork == null) return false;

            artwork.CoverImageUrl = imageUrl;
            artwork.ModifiedAt = DateTime.UtcNow;
            return true;
        }
    }
}