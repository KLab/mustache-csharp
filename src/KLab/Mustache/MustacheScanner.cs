namespace KLab.Mustache
{
    /// <summary>
    /// Mustache template scanner class.
    /// </summary>
    class MustacheScanner
    {
        /// <summary>
        /// Target template.
        /// </summary>
        /// <returns>Mustache template string.</returns>
        public string Source { get; private set; }

        /// <summary>
        /// Current reading position.
        /// </summary>
        /// <returns>Current reading position.</returns>
        public int Pos { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="str">Mustache template string.</param>
        public MustacheScanner(string template)
        {
            Source = template;
            Pos = 0;
        }

        /// <summary>
        /// Returns the character at relative offset.
        /// Current position is not changed.
        /// </summary>
        /// <param name="offset">Relative offset.</param>
        /// <returns>The character at the position, or 0 if calculated position is out of range.</returns>
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
        /// Returns the character at current position.
        /// </summary>
        /// <returns>The character at current position.</returns>
        public char Peek()
        {
            return Peek(0);
        }

        /// <summary>
        /// Moves current position.
        /// </summary>
        /// <param name="offset">Offset that relative to the current position.</param>
        /// <returns><see langword="true"/> if position was updated otherwise <see langword="false"/></returns>
        public bool Seek(int offset)
        {
            var p = Pos + offset;
            if (0 <= p && p < Source.Length)
            {
                Pos = p;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads until just before the pattern found.
        /// </summary>
        /// <param name="pattern">A pattern to find.</param>
        /// <returns>Read string</returns>
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
        /// Seek until just before the pattern found.
        /// </summary>
        /// <param name="pattern">A pattern to find.</param>
        /// <returns>Moved offset</returns>
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
        /// Check if current position starts with the pattern.
        /// </summary>
        /// <param name="pattern">A pattern string to check.</param>
        /// <returns><see langword="true"/> if it starts with the pattern otherwise <see langword="false"/></returns>
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
