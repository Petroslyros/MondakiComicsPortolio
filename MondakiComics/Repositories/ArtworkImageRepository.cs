using Microsoft.EntityFrameworkCore;
using MondakiComics.Data;
using MondakiComics.Repositories.Interfaces;

namespace MondakiComics.Repositories
{
    public class ArtworkImageRepository : BaseRepository<ArtworkImage>, IArtworkImageRepository
    {
        public ArtworkImageRepository(MondakiDbContext context) : base(context) { }

        public async Task<IEnumerable<ArtworkImage>> GetByArtworkIdAsync(int artworkId)
        {
            return await context.ArtworkImages
                .Where(i => i.ArtworkId == artworkId)
                .OrderBy(i => i.SortOrder)
                .ToListAsync();
        }

        public async Task DeleteByArtworkIdAsync(int artworkId)
        {
            var images = await context.ArtworkImages
                .Where(i => i.ArtworkId == artworkId)
                .ToListAsync();

            context.ArtworkImages.RemoveRange(images);
        }

        public async Task<int> GetNextSortOrderAsync(int artworkId)
        {
            var maxOrder = await context.ArtworkImages
                .Where(i => i.ArtworkId == artworkId)
                .MaxAsync(i => (int?)i.SortOrder);

            // If no images yet maxOrder is null, start at 1
            return (maxOrder ?? 0) + 1;
        }
    }
}