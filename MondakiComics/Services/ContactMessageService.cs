using AutoMapper;
using MondakiComics.Data;
using MondakiComics.DTO;
using MondakiComics.Exceptions;
using MondakiComics.Repositories.Interfaces;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Services
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<ContactMessageService> logger;

        public ContactMessageService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<ContactMessageService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<bool> SendMessageAsync(ContactMessageInsertDTO dto)
        {
            // Find the admin user to link the message to
            var adminUser = await unitOfWork.UserRepository.GetAdminUserAsync()
                ?? throw new EntityNotFoundException("User", "No admin user found");

            var message = new ContactMessage
            {
                UserId = adminUser.Id,
                SenderName = dto.SenderName,
                SenderEmail = dto.SenderEmail,
                Message = dto.Message,
                IsRead = false,
                ReceivedAt = DateTime.UtcNow
            };

            await unitOfWork.ContactMessageRepository.AddAsync(message);
            await unitOfWork.SaveAsync();

            logger.LogInformation("New contact message from {SenderEmail}", dto.SenderEmail);
            return true;
        }

        public async Task<IEnumerable<ContactMessageReadOnlyDTO>> GetAllMessagesAsync()
        {
            var messages = await unitOfWork.ContactMessageRepository.GetAllAsync();
            return mapper.Map<IEnumerable<ContactMessageReadOnlyDTO>>(messages);
        }

        public async Task<ContactMessageReadOnlyDTO?> GetMessageByIdAsync(int id)
        {
            var message = await unitOfWork.ContactMessageRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("ContactMessage", $"Message with ID {id} not found");

            return mapper.Map<ContactMessageReadOnlyDTO>(message);
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var message = await unitOfWork.ContactMessageRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("ContactMessage", $"Message with ID {id} not found");

            message.IsRead = true;

            await unitOfWork.ContactMessageRepository.Update(message);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Message {Id} marked as read", id);
            return true;
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await unitOfWork.ContactMessageRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("ContactMessage", $"Message with ID {id} not found");

            await unitOfWork.ContactMessageRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Message deleted: {Id}", id);
            return true;
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await unitOfWork.ContactMessageRepository.GetUnreadCountAsync();
        }
    }
}