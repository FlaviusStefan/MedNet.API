namespace MedNet.API.Models.DTO
{
    public class AddressDto
    {
        public Guid Id { get; set; }
        public string Street { get; set; }
        public int StreetNr { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }
}
