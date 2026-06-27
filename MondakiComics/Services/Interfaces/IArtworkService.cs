using Microsoft.AspNetCore.Http;
using MondakiComics.DTO;

namespace MondakiComics.Services.Interfaces
{
    public interface IArtworkService
    {
        // Public (no auth needed)
        Task<IEnumerable<ArtworkReadOnlyDTO>> GetPublishedArtworksAsync();
        Task<IEnumerable<ArtworkReadOnlyDTO>> GetArtworksByCategoryAsync(int categoryId);
        Task<ArtworkReadOnlyDTO?> GetArtworkByIdAsync(int id);

        // Admin only
        Task<IEnumerable<ArtworkReadOnlyDTO>> GetAllArtworksForAdminAsync();
        Task<ArtworkReadOnlyDTO> CreateArtworkAsync(int userId, ArtworkInsertDTO dto);
        Task<ArtworkReadOnlyDTO> UpdateArtworkAsync(int id, ArtworkUpdateDTO dto);
        Task<bool> DeleteArtworkAsync(int id);
        Task<bool> TogglePublishAsync(int id);

        // Image management
        Task<ArtworkImageReadOnlyDTO> AddImageToArtworkAsync(int artworkId, IFormFile file);
        Task<bool> DeleteImageAsync(int imageId);
        Task<bool> SetCoverImageAsync(int artworkId, int imageId);
    }
}