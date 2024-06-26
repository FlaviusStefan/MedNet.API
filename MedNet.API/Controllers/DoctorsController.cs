﻿using MedNet.API.Data;
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
        public async Task<IActionResult> CreateDoctor(CreateDoctorRequestDto request)
        {
            // Map DTO -> Domain Model

            var doctor = new Doctor
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Specialization = request.Specialization,
                DateOfBirth = request.DateOfBirth.Date,
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
                DateOfBirth = doctor.DateOfBirth.Date,
                Gender = doctor.Gender
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await doctorRepository.GetAllAsync();

            // Mapping: Domain model to DTO

            var response = new List<DoctorDto>();
            foreach (var doctor in doctors)
            {
                response.Add(new DoctorDto
                {
                    Id = doctor.Id,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Specialization = doctor.Specialization,
                    DateOfBirth = doctor.DateOfBirth,
                    Gender = doctor.Gender
                });
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetDoctorById([FromRoute] Guid id)
        {
            var existingDoctor = await doctorRepository.GetById(id);

            if (existingDoctor is null)
            {
                return NotFound();
            }

            var response = new DoctorDto
            {
                Id = existingDoctor.Id,
                FirstName = existingDoctor.FirstName,
                LastName = existingDoctor.LastName,
                Specialization = existingDoctor.Specialization,
                DateOfBirth = existingDoctor.DateOfBirth,
                Gender = existingDoctor.Gender
            };

            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateDoctor([FromRoute] Guid id, UpdateDoctorRequestDto request)
        {
            var doctor = new Doctor
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Specialization = request.Specialization,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender
            };

            doctor = await doctorRepository.UpdateAsync(doctor);

            if(doctor == null)
            {
                return NotFound();
            }

            var response = new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender
            };

            return Ok(response);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteDoctor([FromRoute] Guid id)
        {
            var doctor = await doctorRepository.DeleteAsync(id);

            if(doctor is null)
            {
                return NotFound();
            }

            var response = new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender
            };

            return Ok(response);
        }
    }
}
