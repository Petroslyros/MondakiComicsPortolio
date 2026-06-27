using AutoMapper;
using Microsoft.AspNetCore.Http;
using MondakiComics.Data;
using MondakiComics.DTO;
using MondakiComics.Exceptions;
using MondakiComics.Repositories.Interfaces;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Services
{
    public class ArtworkService : IArtworkService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IImageUploadService imageUploadService;
        private readonly ILogger<ArtworkService> logger;

        public ArtworkService(IUnitOfWork unitOfWork, IMapper mapper,
            IImageUploadService imageUploadService, ILogger<ArtworkService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.imageUploadService = imageUploadService;
            this.logger = logger;
        }

        public async Task<IEnumerable<ArtworkReadOnlyDTO>> GetPublishedArtworksAsync()
        {
            var artworks = await unitOfWork.ArtworkRepository.GetPublishedAsync();
            return mapper.Map<IEnumerable<ArtworkReadOnlyDTO>>(artworks);
        }

        public async Task<IEnumerable<ArtworkReadOnlyDTO>> GetArtworksByCategoryAsync(int categoryId)
        {
            var artworks = await unitOfWork.ArtworkRepository.GetByCategoryAsync(categoryId);
            return mapper.Map<IEnumerable<ArtworkReadOnlyDTO>>(artworks);
        }

        public async Task<ArtworkReadOnlyDTO?> GetArtworkByIdAsync(int id)
        {
            var artwork = await unitOfWork.ArtworkRepository.GetWithImagesAsync(id)
                ?? throw new EntityNotFoundException("Artwork", $"Artwork with ID {id} not found");

            return mapper.Map<ArtworkReadOnlyDTO>(artwork);
        }

        public async Task<IEnumerable<ArtworkReadOnlyDTO>> GetAllArtworksForAdminAsync()
        {
            var artworks = await unitOfWork.ArtworkRepository.GetAllForAdminAsync();
            return mapper.Map<IEnumerable<ArtworkReadOnlyDTO>>(artworks);
        }

        public async Task<ArtworkReadOnlyDTO> CreateArtworkAsync(int userId, ArtworkInsertDTO dto)
        {
            var artwork = mapper.Map<Artwork>(dto);
            artwork.UserId = userId;

            await unitOfWork.ArtworkRepository.AddAsync(artwork);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Artwork created: {Title} by user {UserId}", artwork.Title, userId);
            return mapper.Map<ArtworkReadOnlyDTO>(artwork);
        }

        public async Task<ArtworkReadOnlyDTO> UpdateArtworkAsync(int id, ArtworkUpdateDTO dto)
        {
            var artwork = await unitOfWork.ArtworkRepository.GetWithImagesAsync(id)
                ?? throw new EntityNotFoundException("Artwork", $"Artwork with ID {id} not found");

            mapper.Map(dto, artwork);
            artwork.ModifiedAt = DateTime.UtcNow;

            await unitOfWork.ArtworkRepository.Update(artwork);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Artwork updated: {Id}", id);
            return mapper.Map<ArtworkReadOnlyDTO>(artwork);
        }

        public async Task<bool> DeleteArtworkAsync(int id)
        {
            var artwork = await unitOfWork.ArtworkRepository.GetWithImagesAsync(id)
                ?? throw new EntityNotFoundException("Artwork", $"Artwork with ID {id} not found");

            // Delete all images from R2 first
            foreach (var image in artwork.Images)
            {
                await imageUploadService.DeleteImageAsync(image.ImageUrl);
            }

            // Soft delete
            artwork.IsDeleted = true;
            artwork.DeletedAt = DateTime.UtcNow;
            await unitOfWork.ArtworkRepository.Update(artwork);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Artwork soft-deleted: {Id}", id);
            return true;
        }

        public async Task<bool> TogglePublishAsync(int id)
        {
            var artwork = await unitOfWork.ArtworkRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("Artwork", $"Artwork with ID {id} not found");

            artwork.IsPublished = !artwork.IsPublished;
            artwork.ModifiedAt = DateTime.UtcNow;

            await unitOfWork.ArtworkRepository.Update(artwork);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Artwork {Id} publish toggled to {Status}", id, artwork.IsPublished);
            return artwork.IsPublished;
        }

        public async Task<ArtworkImageReadOnlyDTO> AddImageToArtworkAsync(int artworkId, IFormFile file)
        {
            var artwork = await unitOfWork.ArtworkRepository.GetAsync(artworkId)
                ?? throw new EntityNotFoundException("Artwork", $"Artwork with ID {artworkId} not found");

            // Upload to R2
            var imageUrl = await imageUploadService.UploadImageAsync(file, "artworks");

            // Get next sort order
            var sortOrder = await unitOfWork.ArtworkImageRepository.GetNextSortOrderAsync(artworkId);

            var image = new ArtworkImage
            {
                ArtworkId = artworkId,
                ImageUrl = imageUrl,
                AltText = file.FileName,
                SortOrder = sortOrder
            };

            await unitOfWork.ArtworkImageRepository.AddAsync(image);

            // If this is the first image, set it as cover automatically
            if (sortOrder == 1)
            {
                await unitOfWork.ArtworkRepository.UpdateCoverImageAsync(artworkId, imageUrl);
            }

            await unitOfWork.SaveAsync();

            logger.LogInformation("Image added to artwork {ArtworkId}", artworkId);
            return mapper.Map<ArtworkImageReadOnlyDTO>(image);
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var image = await unitOfWork.ArtworkImageRepository.GetAsync(imageId)
                ?? throw new EntityNotFoundException("ArtworkImage", $"Image with ID {imageId} not found");

            // Delete from R2
            await imageUploadService.DeleteImageAsync(image.ImageUrl);

            // Delete from DB
            await unitOfWork.ArtworkImageRepository.DeleteAsync(imageId);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Image deleted: {ImageId}", imageId);
            return true;
        }

        public async Task<bool> SetCoverImageAsync(int artworkId, int imageId)
        {
            var image = await unitOfWork.ArtworkImageRepository.GetAsync(imageId)
                ?? throw new EntityNotFoundException("ArtworkImage", $"Image with ID {imageId} not found");

            if (image.ArtworkId != artworkId)
                throw new EntityForbiddenException("ArtworkImage", "Image does not belong to this artwork");

            await unitOfWork.ArtworkRepository.UpdateCoverImageAsync(artworkId, image.ImageUrl);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Cover image set for artwork {ArtworkId}", artworkId);
            return true;
        }
    }
}