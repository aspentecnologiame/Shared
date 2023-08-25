using System;
using System.Runtime.Serialization;

namespace ICE.GDocs.Domain.GDocs.Validation
{
    [Serializable]
    public class ValidationException : Exception
    {
        public string ErrorCode { get; }
        public ValidationException()
        {
        }

        public ValidationException(string errorCode) : base()
        {
            ErrorCode = errorCode;
        }

        public ValidationException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public ValidationException(string errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
