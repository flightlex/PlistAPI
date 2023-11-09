using System;

namespace PlistAPI.Exceptions
{

    [Serializable]
    public class InvalidPlistRootException : Exception
    {
        public InvalidPlistRootException() { }
        public InvalidPlistRootException(string message) : base(message) { }
        public InvalidPlistRootException(string message, Exception inner) : base(message, inner) { }
        protected InvalidPlistRootException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
