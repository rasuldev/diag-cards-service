using System;

namespace EaisApi.Exceptions
{
    public class CookieNotFoundException: Exception
    {
        public CookieNotFoundException()
        {
        }

        public CookieNotFoundException(string message) : base(message)
        {
        }
    }
}