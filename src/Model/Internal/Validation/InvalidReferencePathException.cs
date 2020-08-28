using System;
using System.Runtime.Serialization;

namespace StatesLanguage.Model.Internal.Validation
{
    [Serializable]
    public class InvalidReferencePathException : ValidationException
    {
        public InvalidReferencePathException()
        {
        }

        public InvalidReferencePathException(string message) : base(message)
        {
        }

        public InvalidReferencePathException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidReferencePathException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}