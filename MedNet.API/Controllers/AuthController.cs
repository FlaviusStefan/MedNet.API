using MedNet.API.Exceptions;
using MedNet.API.Models.DTO.Auth;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNet.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register-patient")]
        public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientDto registerPatientDto)
        {
            _logger.LogInformation("Patient self-registration request received for email: {Email}", registerPatientDto.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid patient registration request for email: {Email}", registerPatientDto.Email);
                return BadRequest(ModelState);
            }

            try
            {
                var token = await _authService.RegisterPatientAsync(registerPatientDto);

                _logger.LogInformation("Patient self-registration successful for email: {Email}", registerPatientDto.Email);

                return CreatedAtAction(nameof(RegisterPatient), new { message = "Registration successful", token });
            }
            catch (CustomException ex)
            {
                _logger.LogError(ex, "CustomException during patient registration for email: {Email}", registerPatientDto.Email);
                return StatusCode(500, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during patient registration for email: {Email}", registerPatientDto.Email);
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-patient-by-admin")]
        public async Task<IActionResult> RegisterPatientByAdmin([FromBody] RegisterPatientByAdminDto registerDto)
        {
            var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Admin {AdminUserId} registering patient for email: {Email}",
                adminUserId, registerDto.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid patient registration request from admin {AdminUserId} for email: {Email}",
                    adminUserId, registerDto.Email);
                return BadRequest(ModelState);
            }

            try
            {
                var message = await _authService.RegisterPatientByAdminAsync(registerDto);

                _logger.LogInformation("Admin {AdminUserId} successfully registered patient: {Email}",
                    adminUserId, registerDto.Email);

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin patient registration by {AdminUserId} for email: {Email}",
                    adminUserId, registerDto.Email);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-doctor-by-admin")]
        public async Task<IActionResult> RegisterDoctorByAdmin([FromBody] RegisterDoctorByAdminDto registerDto)
        {
            var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Admin {AdminUserId} registering doctor for email: {Email}, License: {LicenseNumber}",
                adminUserId, registerDto.Email, registerDto.LicenseNumber);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid doctor registration request from admin {AdminUserId} for email: {Email}",
                    adminUserId, registerDto.Email);
                return BadRequest(ModelState);
            }

            try
            {
                var message = await _authService.RegisterDoctorByAdminAsync(registerDto);

                _logger.LogInformation("Admin {AdminUserId} successfully registered doctor: {Email}, License: {LicenseNumber}",
                    adminUserId, registerDto.Email, registerDto.LicenseNumber);

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin doctor registration by {AdminUserId} for email: {Email}",
                    adminUserId, registerDto.Email);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-hospital-by-admin")]
        public async Task<IActionResult> RegisterHospitalByAdmin([FromBody] RegisterHospitalByAdminDto registerDto)
        {
            var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Admin {AdminUserId} registering hospital: {Name}, Email: {Email}",
                adminUserId, registerDto.Name, registerDto.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid hospital registration request from admin {AdminUserId} for: {Name}",
                    adminUserId, registerDto.Name);
                return BadRequest(ModelState);
            }

            try
            {
                var message = await _authService.RegisterHospitalByAdminAsync(registerDto);

                _logger.LogInformation("Admin {AdminUserId} successfully registered hospital: {Name}",
                    adminUserId, registerDto.Name);

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin hospital registration by {AdminUserId} for: {Name}",
                    adminUserId, registerDto.Name);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email} from IP: {IpAddress}",
                loginDto.Email, HttpContext.Connection.RemoteIpAddress?.ToString());

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login request for email: {Email}", loginDto.Email);
                return BadRequest(ModelState);
            }

            try
            {
                var token = await _authService.LoginAsync(loginDto);

                _logger.LogInformation("Successful login for email: {Email}", loginDto.Email);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed login attempt for email: {Email}", loginDto.Email);
                return Unauthorized(new { message = "Invalid credentials." });
            }
        }
    }
}