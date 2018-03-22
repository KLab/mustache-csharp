using System;

namespace Mustache
{
    public class MustacheException : Exception
    {
        public MustacheException()
        {
        }

        public MustacheException(string message)
            : base(message)
        {
        }

        public MustacheException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}