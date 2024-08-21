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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Address>()
                .HasMany(a => a.Doctors)
                .WithOne(d => d.Address)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Contact>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Contact>()
                .HasMany(c => c.Doctors)
                .WithOne(d => d.Contact)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Address)
                .WithMany(a => a.Doctors)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Contact)
                .WithMany(c => c.Doctors)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
