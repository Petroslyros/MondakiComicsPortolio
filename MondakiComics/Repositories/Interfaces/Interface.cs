using MondakiComics.Data;

namespace MondakiComics.Repositories.Interfaces
{
    public interface INewsPostRepository : IBaseRepository<NewsPost>
    {
        Task<IEnumerable<NewsPost>> GetPublishedAsync();
        Task<IEnumerable<NewsPost>> GetAllForAdminAsync();
    }
}