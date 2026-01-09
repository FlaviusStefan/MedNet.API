using MedNet.API.Exceptions;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MedNet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorHospitalController : ControllerBase
    {
        private readonly IDoctorHospitalService doctorHospitalService;
        private readonly ILogger<DoctorHospitalController> logger;

        public DoctorHospitalController(
            IDoctorHospitalService doctorHospitalService,
            ILogger<DoctorHospitalController> logger)
        {
            this.doctorHospitalService = doctorHospitalService;
            this.logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("bind")]
        public async Task<IActionResult> BindDoctorToHospitalAsync(Guid doctorId, Guid hospitalId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} attempting to bind Doctor {DoctorId} to Hospital {HospitalId}",
                userId, doctorId, hospitalId);

            try
            {
                await doctorHospitalService.BindDoctorToHospitalAsync(doctorId, hospitalId);

                logger.LogInformation("Admin {UserId} successfully bound Doctor {DoctorId} to Hospital {HospitalId}",
                    userId, doctorId, hospitalId);

                return Ok(new { message = "Doctor successfully bound to hospital." });
            }
            catch (CustomException ex)
            {
                logger.LogWarning("Bind operation failed for Admin {UserId}: {Message}", userId, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while Admin {UserId} binding Doctor {DoctorId} to Hospital {HospitalId}",
                    userId, doctorId, hospitalId);
                return StatusCode(500, new { error = "An unexpected error occurred while binding doctor to hospital." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("unbind")]
        public async Task<IActionResult> UnbindDoctorFromHospitalAsync(Guid doctorId, Guid hospitalId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation("Admin {UserId} attempting to unbind Doctor {DoctorId} from Hospital {HospitalId}",
                userId, doctorId, hospitalId);

            try
            {
                await doctorHospitalService.UnbindDoctorFromHospitalAsync(doctorId, hospitalId);

                logger.LogInformation("Admin {UserId} successfully unbound Doctor {DoctorId} from Hospital {HospitalId}",
                    userId, doctorId, hospitalId);

                return Ok(new { message = "Doctor successfully unbound from hospital." });
            }
            catch (CustomException ex)
            {
                logger.LogWarning("Unbind operation failed for Admin {UserId}: {Message}", userId, ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while Admin {UserId} unbinding Doctor {DoctorId} from Hospital {HospitalId}",
                    userId, doctorId, hospitalId);
                return StatusCode(500, new { error = "An unexpected error occurred while unbinding doctor from hospital." });
            }
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("hospital/{hospitalId:guid}/doctors")]
        public async Task<IActionResult> GetDoctorsByHospital(Guid hospitalId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting doctors for Hospital {HospitalId}",
                userId, userRole, hospitalId);

            try
            {
                var doctors = await doctorHospitalService.GetDoctorsByHospitalAsync(hospitalId);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving doctors for Hospital {HospitalId} by user {UserId}",
                    hospitalId, userId);
                return StatusCode(500, new { error = "An error occurred while retrieving doctors." });
            }
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("doctor/{doctorId:guid}/hospitals")]
        public async Task<IActionResult> GetHospitalsByDoctor(Guid doctorId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            logger.LogInformation("User {UserId} with role {Role} requesting hospitals for Doctor {DoctorId}",
                userId, userRole, doctorId);

            try
            {
                var hospitals = await doctorHospitalService.GetHospitalsByDoctorAsync(doctorId);
                return Ok(hospitals);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving hospitals for Doctor {DoctorId} by user {UserId}",
                    doctorId, userId);
                return StatusCode(500, new { error = "An error occurred while retrieving hospitals." });
            }
        }
    }
}
