using System;

namespace Kallithea_Klone
{
    public class MainActionException : Exception
    {
        //  Properties
        //  ==========

        public new string Message { get; }
        public new Exception InnerException { get; }

        //  Constructors
        //  ============

        public MainActionException() : this("")
        {

        }

        public MainActionException(string message) : this(message, null)
        {

        }

        public MainActionException(string message, Exception inner)
        {
            Message = message;
            InnerException = inner;
        }
    }
}
