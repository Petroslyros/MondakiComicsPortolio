using MondakiComics.DTO;
using Microsoft.AspNetCore.Http;

namespace MondakiComics.Services.Interfaces
{
    public interface INewsPostService
    {
        Task<IEnumerable<NewsPostReadOnlyDTO>> GetPublishedNewsAsync();
        Task<NewsPostReadOnlyDTO?> GetNewsByIdAsync(int id);
        Task<IEnumerable<NewsPostReadOnlyDTO>> GetAllNewsForAdminAsync();
        Task<NewsPostReadOnlyDTO> CreateNewsAsync(NewsPostInsertDTO dto);
        Task<NewsPostReadOnlyDTO> UpdateNewsAsync(int id, NewsPostUpdateDTO dto);
        Task<bool> DeleteNewsAsync(int id);
        Task<bool> TogglePublishAsync(int id);
        Task<NewsPostReadOnlyDTO> SetImageAsync(int id, IFormFile file);
    }
}