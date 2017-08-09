using System;

namespace EaisApi.Exceptions
{
    public class SaveCardException: Exception
    {
        public SaveCardException()
        {
        }

        public SaveCardException(string message) : base(message)
        {
        }
    }
}