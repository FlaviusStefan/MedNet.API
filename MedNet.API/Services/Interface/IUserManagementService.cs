using Microsoft.AspNetCore.Identity;

namespace MedNet.API.Services.Interface
{
    public interface IUserManagementService
    {
        Task<IdentityResult> CreateUserAsync(string email, string password);
        Task<IdentityResult> AssignRoleAsync(string email, string role);
        Task<IdentityResult> DeleteUserByIdAsync(string userId);
    }
}
