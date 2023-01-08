using System;

namespace Nitrox.BuildTool 
{
    public class UnableToValidateUnityGameException : Exception
    {
        public UnableToValidateUnityGameException(string message) : base(message)
        {
        }

        public UnableToValidateUnityGameException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}