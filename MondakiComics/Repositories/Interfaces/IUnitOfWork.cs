namespace MondakiComics.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IArtworkRepository ArtworkRepository { get; }
        IArtworkImageRepository ArtworkImageRepository { get; }
        IArtworkCategoryRepository ArtworkCategoryRepository { get; }
        IContactMessageRepository ContactMessageRepository { get; }
        INewsPostRepository NewsPostRepository { get; }

        Task<bool> SaveAsync();
    }
}