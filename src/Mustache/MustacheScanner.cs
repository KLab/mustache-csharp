namespace Mustache
{
    /// <summary>
    /// Mustache template scanner
    /// </summary>
    class MustacheScanner
    {
        /// <summary>
        /// Target template
        /// </summary>
        /// <returns>mustache template string</returns>
        public string Source { get; private set; }

        /// <summary>
        /// Current reading position
        /// </summary>
        /// <returns>current reading position</returns>
        public int Pos { get; private set; }

        /// <summary>
        /// Initialize MustacheScanner
        /// </summary>
        /// <param name="str">mustache template string</param>
        public MustacheScanner(string str)
        {
            Source = str;
            Pos = 0;
        }

        /// <summary>
        /// Return the character at relative offset
        /// Current position is not changed
        /// </summary>
        /// <param name="offset">relative offset</param>
        /// <returns>the character at the position, or 0 if position is out of range</returns>
        public char Peek(int offset)
        {
            var p = Pos + offset;
            if (0 <= p && p < Source.Length)
            {
                return Source[p];
            }
            return (char)0;
        }

        /// <summary>
        /// Return the character at current position
        /// </summary>
        /// <returns>the character at current position</returns>
        public char Peek()
        {
            return Peek(0);
        }

        /// <summary>
        /// Update current position
        /// </summary>
        /// <param name="offset">offset that relative to the current position</param>
        /// <returns>true if position was updated otherwise false</returns>
        public bool Seek(int offset)
        {
            var p = Pos + offset;
            if (0 <= p && p < Source.Length)
            {
                Pos += offset;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Read util just before the pattern found
        /// </summary>
        /// <param name="pattern">a pattern string to find</param>
        /// <returns>read string</returns>
        public string ReadUntilJustBefore(string pattern)
        {
            var prev = Pos;
            for (; Pos < Source.Length; ++Pos)
            {
                if (StartsWith(pattern))
                {
                    break;
                }
            }

            if (prev == Pos)
            {
                return string.Empty;
            }
            return Source.Substring(prev, Pos - prev);
        }

        /// <summary>
        /// Seek util just before the pattern found
        /// </summary>
        /// <param name="pattern">a pattern string to find</param>
        /// <returns>moved offset</returns>
        public int SeekUntilJustBefore(string pattern)
        {
            var prev = Pos;
            for (; Pos < Source.Length; ++Pos)
            {
                if (StartsWith(pattern))
                {
                    break;
                }
            }
            return Pos - prev;
        }

        /// <summary>
        /// Check if current position starts with the pattern
        /// </summary>
        /// <param name="pattern">a pattern string to check</param>
        /// <returns>true if it starts with the pattern otherwise false</returns>
        public bool StartsWith(string pattern)
        {
            for (int i = 0; i < pattern.Length; ++i)
            {
                if (Source.Length <= Pos + i || Source[Pos + i] != pattern[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
