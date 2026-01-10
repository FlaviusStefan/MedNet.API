using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions; // ✅ Add this
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
        private readonly ILogger<AuthService> _logger;


        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, IPatientService patientService, IDoctorService doctorService, IHospitalService hospitalService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _patientService = patientService;
            _doctorService = doctorService;
            _hospitalService = hospitalService;
            _logger = logger;
        }
        public async Task<string> RegisterPatientAsync(RegisterPatientDto registerPatientDto)
        {
            _logger.LogInformation("Patient self-registration attempt for email: {Email}", registerPatientDto.Email);

            var existingUser = await _userManager.FindByEmailAsync(registerPatientDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Patient registration failed - email already exists: {Email}", registerPatientDto.Email);
                throw new CustomException($"An account with email '{registerPatientDto.Email}' already exists.");
            }

            var user = new IdentityUser
            {
                UserName = registerPatientDto.Email,
                Email = registerPatientDto.Email,
                PhoneNumber = registerPatientDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerPatientDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Patient registration failed for {Email}: {Errors}", registerPatientDto.Email, errors);
                throw new Exception(errors);
            }

            _logger.LogInformation("Identity user created successfully for patient: {Email}, UserId: {UserId}",
                registerPatientDto.Email, user.Id);

            try
            {
                await _userManager.AddToRoleAsync(user, UserRole.Patient.ToString());
                _logger.LogDebug("Patient role assigned to user {UserId}", user.Id);

                var createPatientDto = new CreatePatientRequestDto
                {
                    FirstName = registerPatientDto.FirstName,
                    LastName = registerPatientDto.LastName,
                    UserId = user.Id,
                    DateOfBirth = registerPatientDto.DateOfBirth,
                    Gender = registerPatientDto.Gender,
                    Height = registerPatientDto.Height,
                    Weight = registerPatientDto.Weight,
                    Address = registerPatientDto.Address,
                    Contact = new CreateContactRequestDto
                    {
                        Email = registerPatientDto.Email,
                        Phone = registerPatientDto.PhoneNumber
                    }
                };

                var createdPatient = await _patientService.CreatePatientAsync(createPatientDto);

                _logger.LogInformation("Patient self-registration completed successfully - Email: {Email}, UserId: {UserId}, PatientId: {PatientId}",
                    registerPatientDto.Email, user.Id, createdPatient.Id);

                return await GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during patient self-registration for email: {Email}, rolling back user creation", registerPatientDto.Email);
                await _userManager.DeleteAsync(user);
                _logger.LogInformation("Rolled back user creation for failed patient registration: {Email}", registerPatientDto.Email);
                throw new CustomException("An error occurred while registering the patient: " + ex.Message, ex);
            }
        }

        public async Task<string> RegisterPatientByAdminAsync(RegisterPatientByAdminDto registerDto)
        {
            _logger.LogInformation("Admin creating patient account for email: {Email}", registerDto.Email);

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Admin patient registration failed - email already exists: {Email}", registerDto.Email);
                throw new CustomException($"An account with email '{registerDto.Email}' already exists.");
            }

            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Admin patient registration failed for {Email}: {Errors}", registerDto.Email, errors);
                throw new Exception(errors);
            }

            _logger.LogInformation("Identity user created by admin for patient: {Email}, UserId: {UserId}",
                registerDto.Email, user.Id);

            try
            {
                await _userManager.AddToRoleAsync(user, UserRole.Patient.ToString());

                var createPatientDto = new CreatePatientRequestDto
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserId = user.Id,
                    DateOfBirth = registerDto.DateOfBirth,
                    Gender = registerDto.Gender,
                    Height = registerDto.Height,
                    Weight = registerDto.Weight,
                    Address = registerDto.Address,
                    Contact = new CreateContactRequestDto
                    {
                        Email = registerDto.Email,
                        Phone = registerDto.PhoneNumber
                    }
                };

                var createdPatient = await _patientService.CreatePatientAsync(createPatientDto);

                _logger.LogInformation("Patient account created successfully by admin - Email: {Email}, UserId: {UserId}, PatientId: {PatientId}",
                    registerDto.Email, user.Id, createdPatient.Id);

                return "Patient account created successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin patient registration for email: {Email}, rolling back user creation", registerDto.Email);
                await _userManager.DeleteAsync(user);
                _logger.LogInformation("Rolled back user creation for failed admin patient registration: {Email}", registerDto.Email);
                throw;
            }
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Email, 
                loginDto.Password, 
                false,
                true
            );

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Account locked out for email: {Email}", loginDto.Email);
                throw new CustomException("Account is locked due to multiple failed login attempts. Please try again later.");
            }

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", loginDto.Email);
                throw new Exception("Invalid login attempt.");
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user == null)
            {
                _logger.LogError("User not found after successful sign-in for email: {Email}", loginDto.Email);
                throw new Exception("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("Successful login for email: {Email}, UserId: {UserId}, Roles: {Roles}",
                loginDto.Email, user.Id, string.Join(", ", roles));

            return await GenerateJwtToken(user);  
        }

        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            _logger.LogDebug("Generating JWT token for user {UserId}", user.Id);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);  
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),  // ✅ FIXED: Use UtcNow
                signingCredentials: creds);

            _logger.LogDebug("JWT token generated successfully for user {UserId} with roles: {Roles}",
                user.Id, string.Join(", ", roles));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<string> RegisterDoctorAsync(RegisterDoctorDto registerDto)
        {
            _logger.LogWarning("RegisterDoctorAsync called but not implemented");
            throw new NotImplementedException();
        }

        public async Task<string> RegisterDoctorByAdminAsync(RegisterDoctorByAdminDto registerDto)
        {
            _logger.LogInformation("Admin creating doctor account for email: {Email}, License: {LicenseNumber}",
                registerDto.Email, registerDto.LicenseNumber);

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Admin doctor registration failed - email already exists: {Email}", registerDto.Email);
                throw new CustomException($"An account with email '{registerDto.Email}' already exists.");
            }

            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Admin doctor registration failed for {Email}: {Errors}", registerDto.Email, errors);
                throw new Exception(errors);
            }

            _logger.LogInformation("Identity user created by admin for doctor: {Email}, UserId: {UserId}",
                registerDto.Email, user.Id);

            try
            {
                await _userManager.AddToRoleAsync(user, UserRole.Doctor.ToString());

                var createDoctorDto = new CreateDoctorRequestDto
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserId = user.Id,
                    DateOfBirth = registerDto.DateOfBirth,
                    Gender = registerDto.Gender,
                    Qualifications = registerDto.Qualifications,
                    YearsOfExperience = registerDto.YearsOfExperience,
                    LicenseNumber = registerDto.LicenseNumber,
                    Address = registerDto.Address,
                    SpecializationIds = registerDto.SpecializationIds,
                    Email = registerDto.Email,
                    Password = registerDto.Password,
                    Contact = new CreateContactRequestDto
                    {
                        Email = registerDto.Email,
                        Phone = registerDto.PhoneNumber
                    }
                };

                var createdDoctor = await _doctorService.CreateDoctorAsync(createDoctorDto);

                _logger.LogInformation("Doctor account created successfully by admin - Email: {Email}, UserId: {UserId}, DoctorId: {DoctorId}, License: {LicenseNumber}",
                    registerDto.Email, user.Id, createdDoctor.Id, registerDto.LicenseNumber);

                return "Doctor account created successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor profile for {Email}, rolling back user creation", registerDto.Email);
                await _userManager.DeleteAsync(user);
                _logger.LogInformation("Rolled back user creation for failed doctor registration: {Email}", registerDto.Email);
                throw;
            }
        }

        public async Task<string> RegisterHospitalByAdminAsync(RegisterHospitalByAdminDto registerDto)
        {
            _logger.LogInformation("Admin creating hospital account for: {Name}, Email: {Email}",
                registerDto.Name, registerDto.Email);

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Admin hospital registration failed - email already exists: {Email}", registerDto.Email);
                throw new CustomException($"An account with email '{registerDto.Email}' already exists.");
            }

            // ✅ Start distributed transaction (covers both DbContexts)
            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled);

            try
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
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Admin hospital registration failed for {Name}: {Errors}", registerDto.Name, errors);
                    throw new Exception(errors);
                }

                _logger.LogInformation("Identity user created by admin for hospital: {Name}, UserId: {UserId}",
                    registerDto.Name, user.Id);

                var createHospitalDto = new CreateHospitalRequestDto
                {
                    Name = registerDto.Name,
                    UserId = user.Id,
                    Address = registerDto.Address,
                    Contact = new CreateContactRequestDto
                    {
                        Email = registerDto.Email,
                        Phone = registerDto.PhoneNumber
                    }
                };

                var createdHospital = await _hospitalService.CreateHospitalAsync(createHospitalDto);

                // ✅ Commit BOTH transactions atomically
                scope.Complete();

                _logger.LogInformation("Hospital account created successfully by admin - Name: {Name}, UserId: {UserId}, HospitalId: {HospitalId}",
                    registerDto.Name, user.Id, createdHospital.Id);

                return "Hospital account created successfully.";
            }
            catch (Exception ex)
            {
                // ✅ Both transactions will be rolled back automatically (no manual cleanup needed)
                _logger.LogError(ex, "Error creating hospital profile for {Name}, transaction rolled back", registerDto.Name);
                throw new CustomException("An unexpected error occurred while creating the hospital account: " + ex.Message, ex);
            }
        }
    }
}