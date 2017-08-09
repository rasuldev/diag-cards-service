using System;

namespace EaisApi.Exceptions
{
    public class CheckCardException: Exception
    {
        public CheckResults CheckResult { get; }

        public CheckCardException(CheckResults checkResult)
        {
            CheckResult = checkResult;
        }
    }
}