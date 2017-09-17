using System;

namespace WebUI.Infrastructure
{
    public class RegisterCardException: Exception
    {
        public RegisterCardException()
        {
        }

        public RegisterCardException(string message) : base(message)
        {
        }

        public RegisterCardException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}