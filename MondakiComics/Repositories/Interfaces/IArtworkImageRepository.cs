using MondakiComics.Data;

namespace MondakiComics.Repositories.Interfaces
{
    public interface IArtworkImageRepository : IBaseRepository<ArtworkImage>
    {
        Task<IEnumerable<ArtworkImage>> GetByArtworkIdAsync(int artworkId);
        Task DeleteByArtworkIdAsync(int artworkId);              // delete all images of an artwork
        Task<int> GetNextSortOrderAsync(int artworkId);          // auto increment sort order
    }
}
