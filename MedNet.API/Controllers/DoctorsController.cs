using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;

        public DoctorsController(IDoctorRepository doctorRepository)
        {
            this.doctorRepository = doctorRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateDoctorRequestDto request)
        {
            // Map DTO -> Domain Model

            var doctor = new Doctor
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Specialization = request.Specialization,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
            };

            await doctorRepository.CreateAsync(doctor);

            // Domain Model to DTO
            var response = new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
                Gender = doctor.Gender
            };

            return Ok(response);
        }
    }
}
