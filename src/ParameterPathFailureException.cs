using System;
using System.Runtime.Serialization;

namespace StatesLanguage
{
    [Serializable]
    public class ParameterPathFailureException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ParameterPathFailureException()
        {
        }

        public ParameterPathFailureException(string message) : base(message)
        {
        }

        public ParameterPathFailureException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ParameterPathFailureException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}