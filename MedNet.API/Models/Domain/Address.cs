namespace MedNet.API.Models.Domain
{
    public class Address
    {
        public Guid Id { get; set; }
        public string Street { get; set; }
        public int StreetNr { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public ICollection<Doctor> Doctors { get; set; }
        public ICollection<Patient> Patients { get; set; }
    }
}
