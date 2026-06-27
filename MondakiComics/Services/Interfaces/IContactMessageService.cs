using MondakiComics.DTO;

namespace MondakiComics.Services.Interfaces
{
    public interface IContactMessageService
    {
        // Public - anyone can send a message
        Task<bool> SendMessageAsync(ContactMessageInsertDTO dto);

        // Admin only
        Task<IEnumerable<ContactMessageReadOnlyDTO>> GetAllMessagesAsync();
        Task<ContactMessageReadOnlyDTO?> GetMessageByIdAsync(int id);
        Task<bool> MarkAsReadAsync(int id);
        Task<bool> DeleteMessageAsync(int id);
        Task<int> GetUnreadCountAsync();
    }
}