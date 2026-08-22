using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MondakiComics.DTO;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Controllers
{
    public class NewsController : BaseController
    {
        public NewsController(IApplicationService applicationService) : base(applicationService) { }

        // Public
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsPostReadOnlyDTO>>> GetPublishedNews()
        {
            var news = await applicationService.NewsPostService.GetPublishedNewsAsync();
            return Ok(news);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsPostReadOnlyDTO>> GetNewsById(int id)
        {
            var post = await applicationService.NewsPostService.GetNewsByIdAsync(id);
            return Ok(post);
        }

        // Admin
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<NewsPostReadOnlyDTO>>> GetAllNewsAdmin()
        {
            var news = await applicationService.NewsPostService.GetAllNewsForAdminAsync();
            return Ok(news);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NewsPostReadOnlyDTO>> CreateNews([FromBody] NewsPostInsertDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var post = await applicationService.NewsPostService.CreateNewsAsync(dto);
            return CreatedAtAction(nameof(GetNewsById), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NewsPostReadOnlyDTO>> UpdateNews(int id, [FromBody] NewsPostUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var post = await applicationService.NewsPostService.UpdateNewsAsync(id, dto);
            return Ok(post);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteNews(int id)
        {
            await applicationService.NewsPostService.DeleteNewsAsync(id);
            return Ok(new { message = $"News post {id} deleted successfully" });
        }

        [HttpPatch("{id}/toggle-publish")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> TogglePublish(int id)
        {
            var isPublished = await applicationService.NewsPostService.TogglePublishAsync(id);
            return Ok(new { isPublished });
        }

        [HttpPost("{id}/image")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NewsPostReadOnlyDTO>> SetImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            var post = await applicationService.NewsPostService.SetImageAsync(id, file);
            return Ok(post);
        }
    }
}