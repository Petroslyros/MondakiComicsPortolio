using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MondakiComics.DTO;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Controllers
{
    public class ArtworksController : BaseController
    {
        public ArtworksController(IApplicationService applicationService)
            : base(applicationService) { }

        // Public endpoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArtworkReadOnlyDTO>>> GetPublishedArtworks()
        {
            var artworks = await applicationService.ArtworkService.GetPublishedArtworksAsync();
            return Ok(artworks);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ArtworkReadOnlyDTO>> GetArtworkById(int id)
        {
            var artwork = await applicationService.ArtworkService.GetArtworkByIdAsync(id);
            return Ok(artwork);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ArtworkReadOnlyDTO>>> GetArtworksByCategory(int categoryId)
        {
            var artworks = await applicationService.ArtworkService.GetArtworksByCategoryAsync(categoryId);
            return Ok(artworks);
        }

        // Admin only endpoints
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ArtworkReadOnlyDTO>>> GetAllArtworksAdmin()
        {
            var artworks = await applicationService.ArtworkService.GetAllArtworksForAdminAsync();
            return Ok(artworks);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ArtworkReadOnlyDTO>> CreateArtwork([FromBody] ArtworkInsertDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var artwork = await applicationService.ArtworkService.CreateArtworkAsync(AppUser!.Id, dto);
            return CreatedAtAction(nameof(GetArtworkById), new { id = artwork.Id }, artwork);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ArtworkReadOnlyDTO>> UpdateArtwork(
            int id, [FromBody] ArtworkUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var artwork = await applicationService.ArtworkService.UpdateArtworkAsync(id, dto);
            return Ok(artwork);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteArtwork(int id)
        {
            await applicationService.ArtworkService.DeleteArtworkAsync(id);
            return Ok(new { message = $"Artwork {id} deleted successfully" });
        }

        [HttpPatch("{id}/toggle-publish")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> TogglePublish(int id)
        {
            var isPublished = await applicationService.ArtworkService.TogglePublishAsync(id);
            return Ok(new { isPublished });
        }

        // Image management
        [HttpPost("{artworkId}/images")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ArtworkImageReadOnlyDTO>> AddImage(
            int artworkId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            var image = await applicationService.ArtworkService.AddImageToArtworkAsync(artworkId, file);
            return Ok(image);
        }

        [HttpDelete("{artworkId}/images/{imageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteImage(int artworkId, int imageId)
        {
            await applicationService.ArtworkService.DeleteImageAsync(imageId);
            return Ok(new { message = $"Image {imageId} deleted successfully" });
        }

        [HttpPatch("{artworkId}/images/{imageId}/set-cover")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetCoverImage(int artworkId, int imageId)
        {
            await applicationService.ArtworkService.SetCoverImageAsync(artworkId, imageId);
            return Ok(new { message = "Cover image updated" });
        }
    }
}