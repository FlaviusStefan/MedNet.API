using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MedNet.API.Services.Implementation
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(UserManager<IdentityUser> userManager, ILogger<UserManagementService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IdentityResult> CreateUserAsync(string email, string password)
        {
            _logger.LogInformation("Creating Identity user for email: {Email}", email);

            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Identity user created successfully - Email: {Email}, UserId: {UserId}",
                    email, user.Id);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to create Identity user for {Email}: {Errors}", email, errors);
            }

            return result;
        }

        public async Task<IdentityResult> AssignRoleAsync(string email, string role)
        {
            _logger.LogInformation("Assigning role '{Role}' to user: {Email}", role, email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Cannot assign role - User not found: {Email}", email);
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var result = await _userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Role '{Role}' assigned successfully to user {Email} (UserId: {UserId})",
                    role, email, user.Id);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to assign role '{Role}' to user {Email}: {Errors}", role, email, errors);
            }

            return result;
        }

        public async Task<IdentityResult> DeleteUserByIdAsync(string userId)
        {
            _logger.LogInformation("Deleting Identity user with ID: {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Cannot delete user - User not found with ID: {UserId}", userId);
                throw new Exception($"User with ID {userId} not found.");
            }

            var email = user.Email;
            var userName = user.UserName;

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("Identity user deleted successfully - UserId: {UserId}, Email: {Email}",
                    userId, email);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to delete Identity user {UserId}: {Errors}", userId, errors);
            }

            return result;
        }
    }
}