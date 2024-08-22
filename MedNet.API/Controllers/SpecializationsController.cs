﻿using MedNet.API.Models.DTO;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationsController : ControllerBase
    {
        private readonly ISpecializationService specializationService;

        public SpecializationsController(ISpecializationService specializationService)
        {
            this.specializationService = specializationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpecialization(CreateSpecializationRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var specializationDto = await specializationService.CreateSpecializationAsync(request);
                return CreatedAtAction(nameof(GetSpecializationById), new { id = specializationDto.Id }, specializationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the specialization.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpecializations()
        {
            var specializations = await specializationService.GetAllSpecializationsAsync();
            return Ok(specializations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecializationById(Guid id)
        {
            var specializationDto = await specializationService.GetSpecializationByIdAsync(id);

            if (specializationDto == null)
            {
                return NotFound();
            }

            return Ok(specializationDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpecialization(Guid id, UpdateSpecializationRequestDto request)
        {
            var response = await specializationService.UpdateSpecializationAsync(id, request);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteSpecialization(Guid id)
        {
            var response = await specializationService.DeleteSpecializationAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}