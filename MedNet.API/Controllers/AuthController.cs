using MedNet.API.Exceptions;
using MedNet.API.Models.DTO.Auth;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-patient")]
        public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientDto registerPatientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var token = await _authService.RegisterPatientAsync(registerPatientDto);
                return CreatedAtAction(nameof(RegisterPatient), new { message = "Registration successful", token });
            }
            catch (CustomException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-patient-by-admin")]
        public async Task<IActionResult> RegisterPatientByAdmin([FromBody] RegisterPatientByAdminDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var message = await _authService.RegisterPatientByAdminAsync(registerDto);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-doctor-by-admin")]
        public async Task<IActionResult> RegisterDoctorByAdmin([FromBody] RegisterDoctorByAdminDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var message = await _authService.RegisterDoctorByAdminAsync(registerDto);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-hospital-by-admin")]
        public async Task<IActionResult> RegisterHospitalByAdmin([FromBody] RegisterHospitalByAdminDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var message = await _authService.RegisterHospitalByAdminAsync(registerDto);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _authService.LoginAsync(loginDto);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            return Ok(new { token });
        }

    }
}
