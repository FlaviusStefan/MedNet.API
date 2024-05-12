using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository patientRepository;

        public PatientsController(IPatientRepository patientRepository)
        {
            this.patientRepository = patientRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient(CreatePatientRequestDto request)
        {
            var patient = new Patient
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender
            };

            await patientRepository.CreateAsync(patient);

            var response = new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await patientRepository.GetAllAsync();

            // Mapping: Domain model to DTO 
            var response = new List<PatientDto>();
            foreach(var patient in patients)
            {
                response.Add(new PatientDto
                {
                    Id = patient.Id,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    DateOfBirth = patient.DateOfBirth,
                    Gender = patient.Gender
                });
            }

            return Ok(response);
        }
    }
}
