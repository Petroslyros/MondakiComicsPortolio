using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using MondakiComics.Core.Enums;
using MondakiComics.Core.Filters;
using MondakiComics.Data;
using MondakiComics.DTO;
using MondakiComics.Exceptions;
using MondakiComics.Models;
using MondakiComics.Repositories.Interfaces;
using MondakiComics.Security;
using MondakiComics.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace MondakiComics.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<UserService> logger;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<UserReadOnlyDTO?> GetUserByIdAsync(int id)
        {
            User? user = null;

            try
            {
                user = await unitOfWork.UserRepository.GetAsync(id);
                logger.LogInformation("User found with ID : {Id} ", id);
            }
            catch (Exception ex)
            {
                logger.LogError("Error retrieving user by ID : {Id}. {Mesasge}", id, ex.Message);

            }
            return mapper.Map<UserReadOnlyDTO>(user);
        }

        //public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize,
        //    UserFiltersDTO userFiltersDTO)
        //{
        //    List<User> users = [];
        //    List<Expression<Func<User, bool>>> predicates = [];


        //    if (!string.IsNullOrEmpty(userFiltersDTO.UserName))
        //    {
        //        predicates.Add(u => u.Username == userFiltersDTO.UserName);
        //    }
        //    if (!string.IsNullOrEmpty(userFiltersDTO.Email))
        //    {
        //        predicates.Add(u => u.Email == userFiltersDTO.Email);
        //    }
        //    if (!string.IsNullOrEmpty(userFiltersDTO.UserRole))
        //    {
        //        predicates.Add(u => u.UserRole.ToString() == userFiltersDTO.UserRole);
        //    }

        //    var result = await unitOfWork.UserRepository.GetUsersAsync(pageNumber, pageSize, predicates);
        //    var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
        //    {
        //        Data = result.Data.Select(u => new UserReadOnlyDTO
        //        {
        //            Id = u.Id,
        //            Username = u.Username,
        //            Email = u.Email,
        //        }).ToList(),
        //        TotalRecords = result.TotalRecords,
        //        PageNumber = result.PageNumber,
        //        PageSize = result.PageSize,
        //    };
        //    logger.LogInformation("Retrieved {Count} users", dtoResult.Data.Count);
        //    return dtoResult;

        //}

        public async Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username)
        {
            var user = await unitOfWork.UserRepository.SearchByUsernameAsync(username);
            if (user == null)
            {
                throw new EntityNotFoundException("User", $"User with username: {username} not found");
            }

            logger.LogInformation("User found {Username}", username);
            return mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            User? user = null;
            try
            {
                user = await unitOfWork.UserRepository.GetUserAsync(credentials.Username!, credentials.Password!);

                if (user == null)
                {
                    throw new EntityNotAuthorizedException("User", "Bad Credentials");
                }
                logger.LogInformation("User with username { Username} found", credentials.Username!);
            }
            catch (EntityNotAuthorizedException ex)
            {
                logger.LogError("Authentication failed for username {Username}. {Message}", credentials.Username, ex.Message);
            }
            return user;
        }

        public async Task<UserReadOnlyDTO> RegisterUserAsync(UserRegisterDTO userDto)
        {
            if (await unitOfWork.UserRepository.GetUserByUsernameAsync(userDto.Username!) is not null)
                throw new EntityAlreadyExistsException("User", "Username already exists");

            if (await unitOfWork.UserRepository.EmailExistsAsync(userDto.Email))
                throw new EntityAlreadyExistsException("User", "Email already exists");

            var user = mapper.Map<User>(userDto);


            user.Password = EncryptionUtil.Encrypt(userDto.Password!);
            user.UserRole = UserRole.Admin;

            await unitOfWork.UserRepository.AddAsync(user);
            await unitOfWork.SaveAsync();

            logger.LogInformation("User registered successfully: {Username}", user.Username);

            return mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            // Get the user
            var user = await unitOfWork.UserRepository.GetAsync(id);
            if (user == null) throw new EntityNotFoundException("User", $"User with ID {id} not found");

            // Soft delete: mark as deleted
            user.IsDeleted= true;
            user.DeletedAt = DateTime.UtcNow;

            // Save changes
            await unitOfWork.SaveAsync();

            logger.LogInformation("User soft-deleted successfully with ID {UserId}", id);

            return true;
        }

        //public async Task<UserReadOnlyDTO> UpdateUserAsync(int userId, UpdateUserDTO dto)
        //{
        //    // Get the existing user
        //    var user = await unitOfWork.UserRepository.GetAsync(userId)
        //        ?? throw new EntityNotFoundException("User", $"User with ID {userId} not found");

        //    // Handle email change
        //    if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        //    {
        //        if (await unitOfWork.UserRepository.EmailExistsAsync(dto.Email))
        //            throw new EntityAlreadyExistsException("User", $"Email '{dto.Email}' is already in use.");
        //    }
        //    else
        //    {
        //        dto.Email = null; // prevent AutoMapper from overwriting existing email
        //    }

        //    // Map DTO to entity (other fields only)
        //    mapper.Map(dto, user);

        //    // Update in DB
        //    await unitOfWork.UserRepository.Update(user);
        //    await unitOfWork.SaveAsync();

        //    logger.LogInformation("User updated successfully: {UserId}", user.Id);

        //    // Return mapped DTO
        //    return mapper.Map<UserReadOnlyDTO>(user);
        //}

        public async Task<PaginatedResult<UserReadOnlyDTO>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var result = await unitOfWork.UserRepository.GetUsersAsync(pageNumber, pageSize, new List<Expression<Func<User, bool>>>());

            // AutoMapper will handle mapping List<User> → List<UserReadOnlyDTO>
            var dtoData = mapper.Map<List<UserReadOnlyDTO>>(result.Data);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>
            {
                Data = dtoData,
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

            logger.LogInformation("Retrieved {Count} users from page {PageNumber}", dtoResult.Data.Count, dtoResult.PageNumber);

            return dtoResult;
        }




        /// <summary>
        /// Creates a JWT token for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="email">The email address of the user.</param>
        /// <param name="userRole">The role assigned to the user.</param>
        /// <param name="appSecurityKey">The secret key used to sign the token.</param>
        /// <returns>A signed JWT token string.</returns>
        /// <remarks>
        /// The token includes claims for username, user ID, email, and role.
        /// The issuer and audience are set to example URLs; use <c>null</c> if not needed.
        /// </remarks>
        /// 
        public string CreateUserToken(int userId, string username, string email, UserRole userRole, string appSecurityKey, string issuer)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsInfo = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, userRole.ToString()!)
    };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: issuer,      // 👈 πλέον δυναμικό
                audience: issuer,    // 👈 πλέον δυναμικό
                claims: claimsInfo,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: signingCredentials
            );

            var userToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return userToken;
        }




    }
}
