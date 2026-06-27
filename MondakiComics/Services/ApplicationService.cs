using AutoMapper;
using MondakiComics.Repositories.Interfaces;
using MondakiComics.Services;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IImageUploadService imageUploadService;
        private readonly ILogger<ArtworkService> artworkLogger;
        private readonly ILogger<ArtworkCategoryService> categoryLogger;
        private readonly ILogger<UserService> userLogger;
        private readonly ILogger<ContactMessageService> contactLogger;

        public ApplicationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IImageUploadService imageUploadService,  // injected here
            ILogger<ArtworkService> artworkLogger,
            ILogger<ArtworkCategoryService> categoryLogger,
            ILogger<UserService> userLogger,
            ILogger<ContactMessageService> contactLogger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.imageUploadService = imageUploadService;
            this.artworkLogger = artworkLogger;
            this.categoryLogger = categoryLogger;
            this.userLogger = userLogger;
            this.contactLogger = contactLogger;
        }

        public UserService UserService =>
            new UserService(unitOfWork, mapper, userLogger);

        public ArtworkService ArtworkService =>
            new ArtworkService(unitOfWork, mapper, imageUploadService, artworkLogger);

        public ArtworkCategoryService ArtworkCategoryService =>
            new ArtworkCategoryService(unitOfWork, mapper, categoryLogger);

        public ContactMessageService ContactMessageService =>
            new ContactMessageService(unitOfWork, mapper, contactLogger);
    }
}
