using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationsController : ControllerBase
    {
        private readonly ISpecializationRepository specializationRepository;

        public SpecializationsController(ISpecializationRepository specializationRepository)
        {
            this.specializationRepository = specializationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpecialization(CreateSpecializationRequestDto request)
        {
            var specialization = new Specialization
            {
                Name = request.Name
            };

            await specializationRepository.CreateAsync(specialization);

            var response = new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name
            };

            return Ok(response);
        }
    }
}
