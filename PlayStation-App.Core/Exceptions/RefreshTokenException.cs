using System;

namespace PlayStation_App.Core.Exceptions
{
    public class RefreshTokenException : Exception
    {
        public RefreshTokenException()
        {
        }

        public RefreshTokenException(string message)
            : base(message)
        {
        }
    }
}
