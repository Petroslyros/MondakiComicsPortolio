
using MondakiComics.Data;
using MondakiComics.Models;
using System.Linq.Expressions;

namespace MondakiComics.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserAsync(string username, string password);
        Task<List<User>> SearchByUsernameAsync(string username);
        Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
           List<Expression<Func<User, bool>>> predicates);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);

        Task<User?> GetAdminUserAsync();
    }
}
