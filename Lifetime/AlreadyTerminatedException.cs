using System.Runtime.Serialization;

namespace System.Contracts
{
    public class AlreadyTerminatedException : Exception
    {
        public AlreadyTerminatedException()
        {
        }

        public AlreadyTerminatedException(string message) : base(message)
        {
        }

        public AlreadyTerminatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}