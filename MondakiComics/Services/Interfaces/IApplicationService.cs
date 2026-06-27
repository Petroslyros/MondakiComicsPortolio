namespace MondakiComics.Services.Interfaces
{
    public interface IApplicationService
    {
        UserService UserService { get; }
        ArtworkService ArtworkService { get; }
        ArtworkCategoryService ArtworkCategoryService { get; }

        ContactMessageService ContactMessageService { get; }


    }
}
