﻿using System.ComponentModel.DataAnnotations;

namespace MedNet.API.Models.Domain
{
    public class Contact
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Mobile no. is required")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
        public ICollection<Patient> Patients { get; set; }
    }
}
