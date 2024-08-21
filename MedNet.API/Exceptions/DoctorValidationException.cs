namespace MedNet.API.Exceptions
{
    public class DoctorValidationException : Exception
    {
        public DoctorValidationException(string message) : base(message) { }
    }
}
