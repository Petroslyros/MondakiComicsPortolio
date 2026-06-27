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




        //implementation of getter with expression-bodied property
        public IUserRepository UserRepository => new UserRepository(_context);

        public IArtworkRepository ArtworkRepository => new ArtworkRepository(_context);
        public IArtworkCategoryRepository ArtworkCategoryRepository => new ArtworkCategoryRepository(_context);
        public IArtworkImageRepository ArtworkImageRepository => new ArtworkImageRepository(_context);

        public IContactMessageRepository ContactMessageRepository => new ContactMessageRepository(_context);


        //for commit and rolback
        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
