﻿namespace MedNet.API.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message, Exception innerException) : base(message, innerException) { }
    }
}
