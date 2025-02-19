using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedNet.API.Exceptions;
using MedNet.API.Models.DTO;
using MedNet.API.Models.DTO.Auth;
using MedNet.API.Models.Enums;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MedNet.API.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IPatientService _patientService;

        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, IPatientService patientService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _patientService = patientService;
        }
        public async Task<string> RegisterPatientAsync(RegisterPatientDto registerPatientDto)
        {
            try
            {
                // Step 1: Create User in Identity
                var user = new IdentityUser
                {
                    UserName = registerPatientDto.Email,
                    Email = registerPatientDto.Email,
                    PhoneNumber = registerPatientDto.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, registerPatientDto.Password);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                // Step 2: Assign "Patient" Role to User
                await _userManager.AddToRoleAsync(user, UserRole.Patient.ToString());

                // Step 3: Create Patient Profile via PatientService
                var createPatientDto = new CreatePatientRequestDto
                {
                    FirstName = registerPatientDto.FirstName,
                    LastName = registerPatientDto.LastName,
                    UserId = user.Id, // Use Identity user ID
                    DateOfBirth = registerPatientDto.DateOfBirth,
                    Gender = registerPatientDto.Gender,
                    Height = registerPatientDto.Height,
                    Weight = registerPatientDto.Weight,
                    Address = registerPatientDto.Address,
                    Contact = new CreateContactRequestDto // Ensure contact is passed!
                    {
                        Email = registerPatientDto.Email,
                        Phone = registerPatientDto.PhoneNumber
                    }
                };

                // Call PatientService to create a Patient
                var createdPatient = await _patientService.CreatePatientAsync(createPatientDto);

                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                throw new CustomException("An error occurred while registering the patient: " + ex.Message, ex);
            }
        }


        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);

            if (!result.Succeeded)
            {
                throw new Exception("Invalid login attempt.");
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),  // ✅ Set UserId as "sub"
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id) // ✅ Ensure NameIdentifier has UserId
            };

            var roles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public Task<string> RegisterDoctorAsync(RegisterDoctorDto registerDto)
        {
            throw new NotImplementedException();
        }
    }
}