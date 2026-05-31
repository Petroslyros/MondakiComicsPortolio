using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MondakiComics.Data
{
    public class MondakiDbContextFactory : IDesignTimeDbContextFactory<MondakiDbContext>
    {
        public MondakiDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MondakiDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=mondakicomics;Username=mondaki_user;Password=MondakiComics2025!");

            return new MondakiDbContext(optionsBuilder.Options);
        }
    }
}