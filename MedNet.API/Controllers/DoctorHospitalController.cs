using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MedNet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorHospitalController : ControllerBase
    {
        private readonly IDoctorHospitalService doctorHospitalService;

        public DoctorHospitalController(IDoctorHospitalService doctorHospitalService)
        {
            this.doctorHospitalService = doctorHospitalService;
        }

        [HttpPost("bind")]
        public async Task<IActionResult> BindDoctorToHospitalAsync(Guid doctorId, Guid hospitalId)
        {
            try
            {
                await doctorHospitalService.BindDoctorToHospitalAsync(doctorId, hospitalId);
                return Ok("Doctor successfully bound to hospital.");
            }
            catch (Exception ex)
            {
                // Log the exception and return an appropriate error response
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("unbind")]
        public async Task<IActionResult> UnbindDoctorFromHospitalAsync(Guid doctorId, Guid hospitalId)
        {
            try
            {
                await doctorHospitalService.UnbindDoctorFromHospitalAsync(doctorId, hospitalId);
                return Ok("Doctor successfully unbound from hospital.");
            }
            catch (Exception ex)
            {
                // Log the exception and return an appropriate error response
                return BadRequest(ex.Message);
            }
        }
    }
}
