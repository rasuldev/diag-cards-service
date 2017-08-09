using System;

namespace EaisApi.Exceptions
{
    public class NotAuthorizedException: Exception
    {
        public NotAuthorizedException()
        {
        }

        public NotAuthorizedException(string message) : base(message)
        {
        }
    }
}