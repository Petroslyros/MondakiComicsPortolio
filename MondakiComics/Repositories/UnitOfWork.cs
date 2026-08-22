using Microsoft.EntityFrameworkCore;
using MondakiComics.Data;
using MondakiComics.Repositories.Interfaces;

namespace MondakiComics.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MondakiDbContext _context;

        public UnitOfWork(MondakiDbContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository => new UserRepository(_context);

        public IArtworkRepository ArtworkRepository => new ArtworkRepository(_context);
        public IArtworkCategoryRepository ArtworkCategoryRepository => new ArtworkCategoryRepository(_context);
        public IArtworkImageRepository ArtworkImageRepository => new ArtworkImageRepository(_context);

        public IContactMessageRepository ContactMessageRepository => new ContactMessageRepository(_context);

        public INewsPostRepository NewsPostRepository => new NewsPostRepository(_context);

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}