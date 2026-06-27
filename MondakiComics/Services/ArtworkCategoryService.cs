using AutoMapper;
using MondakiComics.Data;
using MondakiComics.DTO;
using MondakiComics.Exceptions;
using MondakiComics.Repositories.Interfaces;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Services
{
    public class ArtworkCategoryService : IArtworkCategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<ArtworkCategoryService> logger;

        public ArtworkCategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ArtworkCategoryService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IEnumerable<ArtworkCategoryReadOnlyDTO>> GetAllCategoriesAsync()
        {
            var categories = await unitOfWork.ArtworkCategoryRepository.GetAllAsync();
            logger.LogInformation("Retrieved {Count} categories", categories.Count());
            return mapper.Map<IEnumerable<ArtworkCategoryReadOnlyDTO>>(categories);
        }

        public async Task<ArtworkCategoryReadOnlyDTO?> GetCategoryByIdAsync(int id)
        {
            var category = await unitOfWork.ArtworkCategoryRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("ArtworkCategory", $"Category with ID {id} not found");

            return mapper.Map<ArtworkCategoryReadOnlyDTO>(category);
        }

        public async Task<ArtworkCategoryReadOnlyDTO> CreateCategoryAsync(ArtworkCategoryInsertDTO dto)
        {
            if (await unitOfWork.ArtworkCategoryRepository.SlugExistsAsync(dto.Slug))
                throw new EntityAlreadyExistsException("ArtworkCategory", $"Slug '{dto.Slug}' already exists");

            var category = mapper.Map<ArtworkCategory>(dto);

            await unitOfWork.ArtworkCategoryRepository.AddAsync(category);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Category created: {Name}", category.Name);
            return mapper.Map<ArtworkCategoryReadOnlyDTO>(category);
        }

        public async Task<ArtworkCategoryReadOnlyDTO> UpdateCategoryAsync(int id, ArtworkCategoryInsertDTO dto)
        {
            var category = await unitOfWork.ArtworkCategoryRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("ArtworkCategory", $"Category with ID {id} not found");

            // Only check slug uniqueness if it actually changed
            if (category.Slug != dto.Slug && await unitOfWork.ArtworkCategoryRepository.SlugExistsAsync(dto.Slug))
                throw new EntityAlreadyExistsException("ArtworkCategory", $"Slug '{dto.Slug}' already exists");

            mapper.Map(dto, category);
            await unitOfWork.ArtworkCategoryRepository.Update(category);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Category updated: {Id}", id);
            return mapper.Map<ArtworkCategoryReadOnlyDTO>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await unitOfWork.ArtworkCategoryRepository.GetAsync(id)
                ?? throw new EntityNotFoundException("ArtworkCategory", $"Category with ID {id} not found");

            await unitOfWork.ArtworkCategoryRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Category deleted: {Id}", id);
            return true;
        }
    }
}