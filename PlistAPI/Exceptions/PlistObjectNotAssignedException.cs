using System;

namespace PlistAPI.Exceptions
{

    [Serializable]
	public class PlistObjectNotAssignedException : Exception
	{
		public PlistObjectNotAssignedException() { }
		public PlistObjectNotAssignedException(string message) : base(message) { }
		public PlistObjectNotAssignedException(string message, Exception inner) : base(message, inner) { }
		protected PlistObjectNotAssignedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
