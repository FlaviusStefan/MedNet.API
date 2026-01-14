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

            // Address configuration - One-to-One relationships
            modelBuilder.Entity<Address>(b =>
            {
                b.HasKey(a => a.Id);
            });

            // Contact configuration - One-to-One relationships
            modelBuilder.Entity<Contact>(b =>
            {
                b.HasKey(c => c.Id);
            });

            // Doctor configuration
            modelBuilder.Entity<Doctor>(b =>
            {
                b.HasKey(d => d.Id);

                b.HasOne(d => d.Address)
                    .WithOne()
                    .HasForeignKey<Doctor>(d => d.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(d => d.Contact)
                    .WithOne()
                    .HasForeignKey<Doctor>(d => d.ContactId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(d => d.DoctorSpecializations)
                    .WithOne(ds => ds.Doctor)
                    .HasForeignKey(ds => ds.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(d => d.DoctorHospitals)
                    .WithOne(dh => dh.Doctor)
                    .HasForeignKey(dh => dh.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(d => d.Appointments)
                    .WithOne(a => a.Doctor)
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Specialization>(b =>
            {
                b.HasKey(s => s.Id);

                b.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                b.Property(s => s.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                b.HasMany(s => s.DoctorSpecializations)
                    .WithOne(ds => ds.Specialization)
                    .HasForeignKey(ds => ds.SpecializationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DoctorSpecialization>()
                .HasKey(ds => new { ds.DoctorId, ds.SpecializationId });

            modelBuilder.Entity<DoctorHospital>()
                .HasKey(dh => new { dh.DoctorId, dh.HospitalId });

            // Hospital Configuration
            modelBuilder.Entity<Hospital>(b =>
            {
                b.HasKey(h => h.Id);

                b.HasOne(h => h.Address)
                    .WithOne()
                    .HasForeignKey<Hospital>(h => h.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(h => h.Contact)
                    .WithOne()
                    .HasForeignKey<Hospital>(h => h.ContactId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(h => h.DoctorHospitals)
                    .WithOne(dh => dh.Hospital)
                    .HasForeignKey(dh => dh.HospitalId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Patient configuration - CASCADE DELETE FOR ALL PATIENT-OWNED DATA
            modelBuilder.Entity<Patient>(b =>
            {
                b.HasKey(p => p.Id);

                b.HasOne(p => p.Address)
                    .WithOne()
                    .HasForeignKey<Patient>(p => p.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(p => p.Contact)
                    .WithOne()
                    .HasForeignKey<Patient>(p => p.ContactId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(p => p.MedicalFiles)
                    .WithOne(mf => mf.Patient)
                    .HasForeignKey(mf => mf.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(p => p.Medications)
                    .WithOne(m => m.Patient)
                    .HasForeignKey(m => m.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(p => p.Insurances)
                    .WithOne(i => i.Patient)
                    .HasForeignKey(i => i.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(p => p.Appointments)
                    .WithOne(a => a.Patient)
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(p => p.LabAnalyses)
                    .WithOne(la => la.Patient)
                    .HasForeignKey(la => la.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Primary key configurations for entities (no duplicate relationship configs)
            modelBuilder.Entity<MedicalFile>()
                .HasKey(mf => mf.Id);

            modelBuilder.Entity<Medication>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Insurance>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Qualification>(b =>
            {
                b.HasKey(q => q.Id);

                b.Property(q => q.Degree)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property(q => q.Institution)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property(q => q.StudiedYears)
                    .IsRequired()
                    .HasColumnType("int");

                b.Property(q => q.YearOfCompletion)
                    .IsRequired()
                    .HasColumnType("int");

                b.HasOne(q => q.Doctor)
                 .WithMany(d => d.Qualifications)
                 .HasForeignKey(q => q.DoctorId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

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