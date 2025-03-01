﻿using MedNet.API.Models.DTO;
using MedNet.API.Services;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabAnalysesController : ControllerBase
    {
        private readonly ILabAnalysisService labAnalysisService;

        public LabAnalysesController(ILabAnalysisService labAnalysisService)
        {
            this.labAnalysisService = labAnalysisService;
        }

        [Authorize(Roles = "Admin,Doctor")] // Admin and Doctor can create
        [HttpPost]
        public async Task<IActionResult> CreateLabAnalysis(CreateLabAnalysisRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await labAnalysisService.CreateLabAnalysisAsync(request);
                return CreatedAtAction(nameof(GetLabAnalysisById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the lab analysis.");
            }
        }

        [Authorize(Roles = "Admin,Doctor,Patient")] 
        [HttpGet]
        public async Task<IActionResult> GetAllLabAnalyses()
        {
            var response = await labAnalysisService.GetAllLabAnalysesAsync();
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetLabAnalysisById(Guid id)
        {
            var response = await labAnalysisService.GetLabAnalysisByIdAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Doctor")] 
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateLabAnalysis(Guid id, UpdateLabAnalysisRequestDto request)
        {
            var response = await labAnalysisService.UpdateLabAnalysisAsync(id, request);
            if(response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")] 
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteLabAnalysis(Guid id)
        {
            var response = await labAnalysisService.DeleteLabAnalysisAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
