namespace MedNet.API.Models.Domain
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        

    }
}
