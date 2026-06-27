using MondakiComics.Data;

namespace MondakiComics.Repositories.Interfaces
{
    public interface IArtworkRepository : IBaseRepository<Artwork>
    {
        Task<IEnumerable<Artwork>> GetPublishedAsync();          // public gallery
        Task<IEnumerable<Artwork>> GetByCategoryAsync(int categoryId);
        Task<Artwork?> GetWithImagesAsync(int id);               // includes ArtworkImages
        Task<IEnumerable<Artwork>> GetAllForAdminAsync();        // includes unpublished
        Task<bool> UpdateCoverImageAsync(int artworkId, string imageUrl);
    }
}
