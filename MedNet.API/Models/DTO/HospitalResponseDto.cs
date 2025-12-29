namespace MedNet.API.Models.DTO
{
    public class HospitalResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public AddressResponseDto Address { get; set; }
        public ContactResponseDto Contact { get; set; }
    }
}
