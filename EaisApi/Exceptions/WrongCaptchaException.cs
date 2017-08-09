using System;

namespace EaisApi.Exceptions
{
    public class WrongCaptchaException: Exception
    {
        public WrongCaptchaException()
        {
        }

        public WrongCaptchaException(string message) : base(message)
        {
        }
    }
}