using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MondakiComics.DTO;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Controllers
{
    public class ContactMessagesController : BaseController
    {
        public ContactMessagesController(IApplicationService applicationService)
            : base(applicationService) { }

        // Public — anyone can send a message
        [HttpPost]
        public async Task<ActionResult> SendMessage([FromBody] ContactMessageInsertDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await applicationService.ContactMessageService.SendMessageAsync(dto);
            return Ok(new { message = "Message sent successfully" });
        }

        // Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ContactMessageReadOnlyDTO>>> GetAllMessages()
        {
            var messages = await applicationService.ContactMessageService.GetAllMessagesAsync();
            return Ok(messages);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ContactMessageReadOnlyDTO>> GetMessageById(int id)
        {
            var message = await applicationService.ContactMessageService.GetMessageByIdAsync(id);
            return Ok(message);
        }

        [HttpPatch("{id}/mark-read")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            await applicationService.ContactMessageService.MarkAsReadAsync(id);
            return Ok(new { message = $"Message {id} marked as read" });
        }

        [HttpGet("unread-count")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var count = await applicationService.ContactMessageService.GetUnreadCountAsync();
            return Ok(new { unreadCount = count });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            await applicationService.ContactMessageService.DeleteMessageAsync(id);
            return Ok(new { message = $"Message {id} deleted successfully" });
        }
    }
}