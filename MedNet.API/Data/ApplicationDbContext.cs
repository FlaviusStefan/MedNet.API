﻿using MedNet.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace MedNet.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }



        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }


    }
}
