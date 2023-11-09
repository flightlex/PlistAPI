using System;

namespace PlistAPI.Exceptions
{

    [Serializable]
    public class CorruptedPlistException : Exception
    {
        public CorruptedPlistException() { }
        public CorruptedPlistException(string message) : base(message) { }
        public CorruptedPlistException(string message, Exception inner) : base(message, inner) { }
        protected CorruptedPlistException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
