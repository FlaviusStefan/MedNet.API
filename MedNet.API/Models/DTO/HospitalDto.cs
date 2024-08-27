namespace MedNet.API.Models.DTO
{
    public class HospitalDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public AddressDto Address { get; set; }
        public ContactDto Contact { get; set; }
    }
}
