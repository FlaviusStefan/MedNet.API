using MedNet.API.Models;
using MedNet.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace MedNet.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<DoctorHospital> DoctorHospitals { get; set; }
        public DbSet<MedicalFile> MedicalFiles { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<LabAnalysis> LabAnalyses { get; set; }
        public DbSet<LabTest> LabTests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DoctorSpecialization>()
                .ToTable("DoctorSpecializations");

            modelBuilder.Entity<DoctorHospital>()
                .ToTable("DoctorHospitals");

            // Address configuration
            modelBuilder.Entity<Address>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Address>()
                .HasMany(a => a.Doctors)
                .WithOne(d => d.Address)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>()
                .HasMany(a => a.Patients)
                .WithOne(d => d.Address)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Address>()
                .HasMany(a => a.Hospitals)
                .WithOne(d => d.Address)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contact configuration
            modelBuilder.Entity<Contact>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Contact>()
                .HasMany(c => c.Doctors)
                .WithOne(d => d.Contact)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contact>()
                .HasMany(c => c.Patients)
                .WithOne(d => d.Contact)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contact>()
                .HasMany(c => c.Hospitals)
                .WithOne(d => d.Contact)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor configuration
            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Address)
                .WithMany(a => a.Doctors)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Contact)
                .WithMany(c => c.Doctors)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.DoctorSpecializations)
                .WithOne(ds => ds.Doctor)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Specialization>()
                .HasMany(s => s.DoctorSpecializations)
                .WithOne(ds => ds.Specialization)
                .HasForeignKey(ds => ds.SpecializationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoctorSpecialization>()
                .HasKey(ds => new { ds.DoctorId, ds.SpecializationId });

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.DoctorHospitals)
                .WithOne(dh => dh.Doctor)
                .HasForeignKey(dh => dh.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Hospital>()
                .HasMany(h => h.DoctorHospitals)
                .WithOne(dh => dh.Hospital)
                .HasForeignKey(dh => dh.HospitalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoctorHospital>()
                .HasKey(dh => new { dh.DoctorId, dh.HospitalId });

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Qualifications)
                .WithOne(q => q.Doctor)
                .HasForeignKey(q => q.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Hospital Configuration
            modelBuilder.Entity<Hospital>()
                .HasKey(h => h.Id);

            modelBuilder.Entity<Hospital>()
                .HasOne(h => h.Address)
                .WithMany(a => a.Hospitals)
                .HasForeignKey(h => h.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hospital>()
                .HasOne(h => h.Contact)
                .WithMany(c => c.Hospitals)
                .HasForeignKey(h => h.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient configuration - CASCADE DELETE FOR ALL PATIENT-OWNED DATA
            modelBuilder.Entity<Patient>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Patient>()
                .HasOne(d => d.Address)
                .WithMany(a => a.Patients)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Patient>()
                .HasOne(d => d.Contact)
                .WithMany(c => c.Patients)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.MedicalFiles)
                .WithOne(mf => mf.Patient)
                .HasForeignKey(mf => mf.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Medications)
                .WithOne(m => m.Patient)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Insurances)
                .WithOne(i => i.Patient)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Appointments)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.LabAnalyses)
                .WithOne(la => la.Patient)
                .HasForeignKey(la => la.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Specialization configuration 
            modelBuilder.Entity<Specialization>()
                .HasKey(s => s.Id);

            // Primary key configurations for entities (no duplicate relationship configs)
            modelBuilder.Entity<MedicalFile>()
                .HasKey(mf => mf.Id);

            modelBuilder.Entity<Medication>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Insurance>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Qualification>()
                .HasKey(q => q.Id);

            // Appointment configuration
            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.Id);

            // Lab Analyses configuration 
            modelBuilder.Entity<LabAnalysis>()
                .HasKey(la => la.Id);

            modelBuilder.Entity<LabAnalysis>()
                .HasMany(la => la.LabTests)
                .WithOne(lt => lt.LabAnalysis)
                .HasForeignKey(lt => lt.LabAnalysisId)
                .OnDelete(DeleteBehavior.Cascade);

            // Lab Tests configuration
            modelBuilder.Entity<LabTest>()
                .HasKey(lt => lt.Id);
        }
    }
}