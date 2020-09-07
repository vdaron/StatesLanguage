using System;
using System.Runtime.Serialization;

namespace StatesLanguage.Internal.Validation
{
    [Serializable]
    public class InvalidIntrinsicFunctionException : ValidationException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidIntrinsicFunctionException()
        {
        }

        public InvalidIntrinsicFunctionException(string message) : base(message)
        {
        }

        public InvalidIntrinsicFunctionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidIntrinsicFunctionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}