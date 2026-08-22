using AutoMapper;
using Microsoft.AspNetCore.Http;
using MondakiComics.DTO;
using MondakiComics.Data;
using MondakiComics.Exceptions;
using MondakiComics.Repositories.Interfaces;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Services
{
    public class NewsPostService : INewsPostService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IImageUploadService imageUploadService;
        private readonly ILogger<NewsPostService> logger;

        public NewsPostService(IUnitOfWork unitOfWork, IMapper mapper, IImageUploadService imageUploadService, ILogger<NewsPostService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.imageUploadService = imageUploadService;
            this.logger = logger;
        }

        public async Task<IEnumerable<NewsPostReadOnlyDTO>> GetPublishedNewsAsync()
        {
            var news = await unitOfWork.NewsPostRepository.GetPublishedAsync();
            return mapper.Map<IEnumerable<NewsPostReadOnlyDTO>>(news);
        }

        public async Task<NewsPostReadOnlyDTO?> GetNewsByIdAsync(int id)
        {
            var post = await unitOfWork.NewsPostRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("NewsPost", $"News post with ID {id} not found");
            return mapper.Map<NewsPostReadOnlyDTO>(post);
        }

        public async Task<IEnumerable<NewsPostReadOnlyDTO>> GetAllNewsForAdminAsync()
        {
            var news = await unitOfWork.NewsPostRepository.GetAllForAdminAsync();
            return mapper.Map<IEnumerable<NewsPostReadOnlyDTO>>(news);
        }

        public async Task<NewsPostReadOnlyDTO> CreateNewsAsync(NewsPostInsertDTO dto)
        {
            var post = mapper.Map<NewsPost>(dto);
            await unitOfWork.NewsPostRepository.AddAsync(post);
            await unitOfWork.SaveAsync();

            logger.LogInformation("News post created: {Title}", post.Title);
            return mapper.Map<NewsPostReadOnlyDTO>(post);
        }

        public async Task<NewsPostReadOnlyDTO> UpdateNewsAsync(int id, NewsPostUpdateDTO dto)
        {
            var post = await unitOfWork.NewsPostRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("NewsPost", $"News post with ID {id} not found");

            mapper.Map(dto, post);
            post.ModifiedAt = DateTime.UtcNow;

            await unitOfWork.NewsPostRepository.Update(post);
            await unitOfWork.SaveAsync();

            logger.LogInformation("News post updated: {Id}", id);
            return mapper.Map<NewsPostReadOnlyDTO>(post);
        }

        public async Task<bool> DeleteNewsAsync(int id)
        {
            var post = await unitOfWork.NewsPostRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("NewsPost", $"News post with ID {id} not found");

            if (!string.IsNullOrEmpty(post.ImageUrl))
            {
                await imageUploadService.DeleteImageAsync(post.ImageUrl);
            }

            post.IsDeleted = true;
            post.DeletedAt = DateTime.UtcNow;
            await unitOfWork.NewsPostRepository.Update(post);
            await unitOfWork.SaveAsync();

            logger.LogInformation("News post soft-deleted: {Id}", id);
            return true;
        }

        public async Task<bool> TogglePublishAsync(int id)
        {
            var post = await unitOfWork.NewsPostRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("NewsPost", $"News post with ID {id} not found");

            post.IsPublished = !post.IsPublished;
            post.ModifiedAt = DateTime.UtcNow;

            await unitOfWork.NewsPostRepository.Update(post);
            await unitOfWork.SaveAsync();

            return post.IsPublished;
        }

        public async Task<NewsPostReadOnlyDTO> SetImageAsync(int id, IFormFile file)
        {
            var post = await unitOfWork.NewsPostRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("NewsPost", $"News post with ID {id} not found");

            // Delete old image if replacing
            if (!string.IsNullOrEmpty(post.ImageUrl))
            {
                await imageUploadService.DeleteImageAsync(post.ImageUrl);
            }

            var imageUrl = await imageUploadService.UploadImageAsync(file, "news");
            post.ImageUrl = imageUrl;
            post.ModifiedAt = DateTime.UtcNow;

            await unitOfWork.NewsPostRepository.Update(post);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Image set for news post {Id}", id);
            return mapper.Map<NewsPostReadOnlyDTO>(post);
        }
    }
}