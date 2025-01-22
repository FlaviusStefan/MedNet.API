namespace MedNet.API.Models.DTO
{
    public class DisplayLabTestDto
    {
        public Guid Id { get; set; }
        public string TestName { get; set; }
        public string Result { get; set; }
        public string Units { get; set; }
        public string ReferenceRange { get; set; }
    }
}
