using System;
using System.Runtime.Serialization;

namespace StatesLanguage
{
    [Serializable]
    public class ResultPathMatchFailureException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ResultPathMatchFailureException()
        {
        }

        public ResultPathMatchFailureException(string message) : base(message)
        {
        }

        public ResultPathMatchFailureException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ResultPathMatchFailureException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}