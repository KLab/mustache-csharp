using System;
using System.Text.RegularExpressions;

namespace Mustache
{
    public class MustacheScanner
    {
        public string Source { get; private set; }

        public int Pos { get; private set; }

        public string Tail
        {
            get
            {
                return (Pos < Source.Length) ? Source.Substring(Pos) : string.Empty;
            }
        }

        public MustacheScanner(string str)
        {
            Source = str;
            Pos = 0;
        }

        public bool Eos()
        {
            return Source.Length <= Pos;
        }

        public char Peek()
        {
            if (Eos())
            {
                return (char)0;
            }
            return Source[Pos];
        }

        public void Seek(int offset)
        {
            Pos += offset;
        }

        public int DiscardWhiteSpaces()
        {
            int read = 0;
            for (; Pos < Source.Length; ++Pos)
            {
                if (!Char.IsWhiteSpace(Source[Pos]))
                {
                    break;
                }
                read++;
            }
            return read;
        }

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
