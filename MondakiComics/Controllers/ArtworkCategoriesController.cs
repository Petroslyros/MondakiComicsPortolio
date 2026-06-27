using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MondakiComics.DTO;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Controllers
{
    public class ArtworkCategoriesController : BaseController
    {
        public ArtworkCategoriesController(IApplicationService applicationService)
            : base(applicationService) { }

        // Public — anyone can see categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArtworkCategoryReadOnlyDTO>>> GetAllCategories()
        {
            var categories = await applicationService.ArtworkCategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArtworkCategoryReadOnlyDTO>> GetCategoryById(int id)
        {
            var category = await applicationService.ArtworkCategoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }

        // Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ArtworkCategoryReadOnlyDTO>> CreateCategory(
            [FromBody] ArtworkCategoryInsertDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await applicationService.ArtworkCategoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ArtworkCategoryReadOnlyDTO>> UpdateCategory(
            int id, [FromBody] ArtworkCategoryInsertDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await applicationService.ArtworkCategoryService.UpdateCategoryAsync(id, dto);
            return Ok(category);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            await applicationService.ArtworkCategoryService.DeleteCategoryAsync(id);
            return Ok(new { message = $"Category {id} deleted successfully" });
        }
    }
}