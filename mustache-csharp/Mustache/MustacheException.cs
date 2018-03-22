using System;

namespace Mustache
{
    /// <summary>
    /// Mustache exception type
    /// </summary>
    public class MustacheException : Exception
    {
        /// <summary>
        /// Initialize
        /// </summary>
        public MustacheException()
        {
        }

        /// <summary>
        /// Initialize with message
        /// </summary>
        /// <param name="message">exception message</param>
        public MustacheException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize with message and inner
        /// </summary>
        /// <param name="message">exception message</param>
        /// <param name="inner">inner exception</param>
        public MustacheException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}