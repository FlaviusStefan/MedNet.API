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
        private readonly IDoctorService _doctorService;
        private readonly IHospitalService _hospitalService;

        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, IPatientService patientService, IDoctorService doctorService, IHospitalService hospitalService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _patientService = patientService;
            _doctorService = doctorService;
            _hospitalService = hospitalService;
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

        public async Task<string> RegisterPatientByAdminAsync(RegisterPatientByAdminDto registerDto)
        {
            // Step 1: Create IdentityUser with the admin-supplied password
            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Step 2: Assign "Patient" Role to the user
            await _userManager.AddToRoleAsync(user, UserRole.Patient.ToString());

            // Step 3: Create Patient Profile via PatientService
            var createPatientDto = new CreatePatientRequestDto
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserId = user.Id,  // Link the IdentityUser to the patient record
                DateOfBirth = registerDto.DateOfBirth,
                Gender = registerDto.Gender,
                Height = registerDto.Height,
                Weight = registerDto.Weight,
                Address = registerDto.Address,
                // Use the same email & phone as in the user registration
                Contact = new CreateContactRequestDto
                {
                    Email = registerDto.Email,
                    Phone = registerDto.PhoneNumber
                }
            };

            await _patientService.CreatePatientAsync(createPatientDto);

            // You could return a success message instead of a token in this case,
            // since the patient might not log in immediately.
            return "Patient account created successfully.";
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

        public async Task<string> RegisterDoctorByAdminAsync(RegisterDoctorByAdminDto registerDto)
        {
            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            try
            {
                // Step 3: Assign "Doctor" Role to the user
                await _userManager.AddToRoleAsync(user, UserRole.Doctor.ToString());

                // Step 4: Create Doctor Profile via DoctorService
                var createDoctorDto = new CreateDoctorRequestDto
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserId = user.Id,
                    DateOfBirth = registerDto.DateOfBirth,
                    Gender = registerDto.Gender,
                    Qualifications = registerDto.Qualifications, // ✅ FIXED: Use Qualifications collection
                    YearsOfExperience = registerDto.YearsOfExperience,
                    LicenseNumber = registerDto.LicenseNumber,
                    Address = registerDto.Address,
                    SpecializationIds = registerDto.SpecializationIds,
                    Email = registerDto.Email, // ✅ ADDED: Required by CreateDoctorRequestDto
                    Password = registerDto.Password, // ✅ ADDED: Required by CreateDoctorRequestDto
                    Contact = new CreateContactRequestDto
                    {
                        Email = registerDto.Email,
                        Phone = registerDto.PhoneNumber
                    }
                };

                await _doctorService.CreateDoctorAsync(createDoctorDto);

                return "Doctor account created successfully.";
            }
            catch (Exception)
            {
                // Rollback user creation if doctor creation fails
                await _userManager.DeleteAsync(user);
                throw;
            }
        }

        public async Task<string> RegisterHospitalByAdminAsync(RegisterHospitalByAdminDto registerDto)
        {
            // Step 1: Create IdentityUser with the admin-supplied password
            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Step 3: Create hospital Profile via hospitalService
            var createHospitalDto = new CreateHospitalRequestDto
            {
                Name = registerDto.Name,
                UserId = user.Id,  // Link the IdentityUser to the hospital record
                Address = registerDto.Address,
                // Use the same email & phone as in the user registration
                Contact = new CreateContactRequestDto
                {
                    Email = registerDto.Email,
                    Phone = registerDto.PhoneNumber
                }
            };

            await _hospitalService.CreateHospitalAsync(createHospitalDto);

            // You could return a success message instead of a token in this case,
            // since the hospital might not log in immediately.
            return "Hospital account created successfully.";
        }
    }
}