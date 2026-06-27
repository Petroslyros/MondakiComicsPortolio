using MondakiComics.DTO;

namespace MondakiComics.Services.Interfaces
{
    public interface IArtworkCategoryService
    {
        Task<IEnumerable<ArtworkCategoryReadOnlyDTO>> GetAllCategoriesAsync();
        Task<ArtworkCategoryReadOnlyDTO?> GetCategoryByIdAsync(int id);
        Task<ArtworkCategoryReadOnlyDTO> CreateCategoryAsync(ArtworkCategoryInsertDTO dto);
        Task<ArtworkCategoryReadOnlyDTO> UpdateCategoryAsync(int id, ArtworkCategoryInsertDTO dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}