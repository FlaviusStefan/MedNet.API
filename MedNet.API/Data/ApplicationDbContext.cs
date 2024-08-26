using MedNet.API.Models.Domain;
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
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DoctorSpecialization>()
                .ToTable("DoctorSpecializations");

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
                .HasForeignKey(ds => ds.DoctorId);

            modelBuilder.Entity<Specialization>()
                .HasMany(s => s.DoctorSpecializations)
                .WithOne(ds => ds.Specialization)
                .HasForeignKey(ds => ds.SpecializationId);


            modelBuilder.Entity<DoctorSpecialization>()
                .HasKey(ds => new { ds.DoctorId, ds.SpecializationId });

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Qualifications)
                .WithOne(q => q.Doctor)
                .HasForeignKey(q => q.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient configuration
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

            // Configuration for Qualification entity
            modelBuilder.Entity<Qualification>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<Qualification>()
                .HasOne(q => q.Doctor)
                .WithMany(d => d.Qualifications)
                .HasForeignKey(q => q.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
