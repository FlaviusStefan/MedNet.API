﻿using MedNet.API.Models.DTO;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QualificationsController : ControllerBase
    {
        private readonly IQualificationService qualificationService;

        public QualificationsController(IQualificationService qualificationService)
        {
            this.qualificationService = qualificationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQualification(CreateQualificationRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var qualificationDto = await qualificationService.CreateQualificationAsync(request);
                return CreatedAtAction(nameof(GetQualificationById), new { id = qualificationDto.Id }, qualificationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the qualification.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQualifications()
        {
            var qualifications = await qualificationService.GetAllQualificationsAsync();
            return Ok(qualifications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQualificationById(Guid id)
        {
            var qualificationDto = await qualificationService.GetQualificationByIdAsync(id);

            if (qualificationDto == null)
            {
                return NotFound();
            }

            return Ok(qualificationDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQualification(Guid id, UpdateQualificationRequestDto request)
        {
            var response = await qualificationService.UpdateQualificationAsync(id, request);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteQualification(Guid id)
        {
            var response = await qualificationService.DeleteQualificationAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
