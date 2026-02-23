using AutoMapper;
using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.User.Queries.GetInactiveUsers;
using HRKošarka.Application.Features.User.Queries.GetNonAdminUsers;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IClubRepository _clubRepository;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, IClubRepository clubRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _clubRepository = clubRepository;
        }

        public async Task<PaginatedResponse<NonAdminUserDTO>> GetNonAdminUsersPagedAsync(GetNonAdminUsersQuery request)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(u =>
                    u.Email!.Contains(term) ||
                    u.FirstName!.Contains(term) ||
                    u.LastName!.Contains(term));
            }

            var allUsers = await query.ToListAsync();

            var nonAdmins = new List<ApplicationUser>();

            foreach (var user in allUsers.Where(x => x.LockoutEnd is null))
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Administrator"))
                {
                    nonAdmins.Add(user);
                }
            }

            var ordered = nonAdmins
                .OrderBy(u => u.Email)
                .ToList();

            var totalCount = ordered.Count;
            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var pageUsers = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = _mapper.Map<List<NonAdminUserDTO>>(pageUsers);

            foreach (var (user, dto) in pageUsers.Zip(dtos))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault() ?? string.Empty;

                dto.Role = primaryRole switch
                {
                    "RegularUser" => "Regular user",
                    "ClubManager" => "Club manager",
                    _ => primaryRole
                };
            }

            await AssignClubNames(dtos);

            return PaginatedResponse<NonAdminUserDTO>.Success(
                dtos,
                page,
                pageSize,
                totalCount,
                "Retrieved non-admin users");
        }

        private async Task AssignClubNames(List<NonAdminUserDTO> dtos)
        {
            var clubIds = dtos
                .Where(u => u.ManagedClubId.HasValue)
                .Select(u => u.ManagedClubId!.Value)
                .Distinct()
                .ToList();

            if (!clubIds.Any())
            {
                return;
            }

            var clubs = await _clubRepository.GetAsync();
            var clubLookup = clubs
                .Where(c => clubIds.Contains(c.Id))
                .ToDictionary(c => c.Id, c => c.Name);

            foreach (var dto in dtos)
            {
                if (dto.ManagedClubId.HasValue &&
                    clubLookup.TryGetValue(dto.ManagedClubId.Value, out var name))
                {
                    dto.ManagedClubName = name;
                }
            }
        }

        public async Task<SimpleResponse> LockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new SimpleResponse { IsSuccess = false, Message = "User not found" };
            }

            try
            {
                var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, "RegularUser");
                if (!removeRoleResult.Succeeded)
                {
                    return new SimpleResponse
                    {
                        IsSuccess = false,
                        Message = "Failed to remove Regular role",
                        Errors = removeRoleResult.Errors.Select(e => e.Description).ToList()
                    };
                }

                // Lock account
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return new SimpleResponse
                    {
                        IsSuccess = false,
                        Message = "Failed to lock user",
                        Errors = updateResult.Errors.Select(e => e.Description).ToList()
                    };
                }

                return new SimpleResponse
                {
                    IsSuccess = true,
                    Message = "User locked successfully"
                };
            }
            catch (Exception ex)
            {
                return new SimpleResponse
                {
                    IsSuccess = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }


        public async Task<SimpleResponse> UnlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new SimpleResponse { IsSuccess = false, Message = "User not found" };
            }

            try
            {
                // Clear lockout
                user.LockoutEnd = null;
                user.LockoutEnabled = false;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return new SimpleResponse
                    {
                        IsSuccess = false,
                        Message = "Failed to unlock user",
                        Errors = updateResult.Errors.Select(e => e.Description).ToList()
                    };
                }

                const string regularRole = "RegularUser";
                if (!await _userManager.IsInRoleAsync(user, regularRole))
                {
                    var addRoleResult = await _userManager.AddToRoleAsync(user, regularRole);
                    if (!addRoleResult.Succeeded)
                    {
                        return new SimpleResponse
                        {
                            IsSuccess = false,
                            Message = "Failed to assign Regular user role",
                            Errors = addRoleResult.Errors.Select(e => e.Description).ToList()
                        };
                    }
                }

                return new SimpleResponse
                {
                    IsSuccess = true,
                    Message = "User unlocked successfully"
                };
            }
            catch (Exception ex)
            {
                return new SimpleResponse
                {
                    IsSuccess = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

        public async Task<PaginatedResponse<InactiveUserDTO>> GeInactiveUsersPagedAsync(GetInactiveUsersQuery request)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(u =>
                    u.Email!.Contains(term) ||
                    u.FirstName!.Contains(term) ||
                    u.LastName!.Contains(term));
            }

            var allUsers = await query.ToListAsync();

            var inactiveUsers = new List<InactiveUserDTO>();

            foreach (var user in allUsers.Where(x => x.LockoutEnd != null))
            {
                inactiveUsers.Add(new InactiveUserDTO
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FirstName + " " + user.LastName
                });
            }

            var totalCount = inactiveUsers.Count;
            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var pageUsers = inactiveUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PaginatedResponse<InactiveUserDTO>.Success(
                pageUsers,
                page,
                pageSize,
                totalCount,
                "Retrieved inactive users");
        }

    }
}
