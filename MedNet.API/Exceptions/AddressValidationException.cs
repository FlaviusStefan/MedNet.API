namespace MedNet.API.Exceptions
{
    public class AddressValidationException : Exception
    {
        public AddressValidationException(string message) : base(message) { }
    }
}
