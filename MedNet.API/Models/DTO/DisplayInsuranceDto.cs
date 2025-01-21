namespace MedNet.API.Models.DTO
{
    public class DisplayInsuranceDto
    {
        public Guid Id { get; set; }
        public string Provider { get; set; }
        public string PolicyNumber { get; set; }
        public DateTime CoverageStartDate { get; set; }
        public DateTime CoverageEndDate { get; set; }
    }
}
