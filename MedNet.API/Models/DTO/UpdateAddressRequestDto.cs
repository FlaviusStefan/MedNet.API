﻿namespace MedNet.API.Models.DTO
{
    public class UpdateAddressRequestDto
    {
        public string Street { get; set; }
        public int StreetNr { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }
}
