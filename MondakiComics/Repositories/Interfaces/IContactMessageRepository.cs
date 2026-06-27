using MondakiComics.Data;

namespace MondakiComics.Repositories.Interfaces
{
    public interface IContactMessageRepository : IBaseRepository<ContactMessage>
    {
        Task<IEnumerable<ContactMessage>> GetAllAsync();
        Task<int> GetUnreadCountAsync();
    }
}