using System;

namespace PlistAPI.Exceptions
{

    [Serializable]
    public class PlistPropertiesNotFoundException : Exception
    {
        public PlistPropertiesNotFoundException() { }
        public PlistPropertiesNotFoundException(string message) : base(message) { }
        public PlistPropertiesNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected PlistPropertiesNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
