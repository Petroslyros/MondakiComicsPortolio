

using MondakiComics.Core.Filters;
using MondakiComics.Data;
using MondakiComics.DTO;
using MondakiComics.Models;

namespace MondakiComics.Services.Interfaces
{
    public interface IUserService
    {
        //
        Task<UserReadOnlyDTO?> GetUserByIdAsync(int id);
        Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username);
        //Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize, UserFiltersDTO userFiltersDTO);
    }
}
