using System;
using System.Text.RegularExpressions;

namespace Mustache
{
    public class MustacheScanner
    {
        public string String { get; private set; }
        public string Tail { get; private set; }
        public int Pos { get; private set; }

        public MustacheScanner()
        {
            String = null;
            Tail = null;
            Pos = 0;
        }

        public MustacheScanner(string str)
        {
            String = str;
            Tail = str;
            Pos = 0;
        }

        public bool Eos()
        {
            return string.IsNullOrEmpty(Tail);
        }

        public string Scan(string pattern)
        {
            var re = new Regex(pattern); // TODO: reuse regex.
            var match = re.Match(Tail);

            if (match.Success && match.Index == 0)
            {
                Tail = Tail.Substring(match.Length);
                Pos += match.Length;
                return match.Value;
            }

            return null;
        }

        public string ScanUntil(string pattern)
        {
            var skipped = string.Empty;
            var re = new Regex(pattern); // TODO: reuse regex.
            var match = re.Match(Tail);

            if (match.Success)
            {
                if (0 < match.Index)
                {
                    skipped = Tail.Substring(0, match.Index);
                    Tail = Tail.Substring(match.Index); // TODO: reduce string allocation.
                    Pos += skipped.Length;
                }
            }
            else
            {
                skipped = Tail;
                Pos += Tail.Length;
                Tail = string.Empty;
            }

            return skipped;
        }
    }
}
