using MondakiComics.Data;

namespace MondakiComics.Repositories.Interfaces
{
    public interface IArtworkCategoryRepository : IBaseRepository<ArtworkCategory>
    {
        Task<ArtworkCategory?> GetBySlugAsync(string slug);
        Task<bool> SlugExistsAsync(string slug);
    }
}
