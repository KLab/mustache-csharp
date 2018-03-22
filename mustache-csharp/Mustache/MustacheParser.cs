using System;
using System.Text;
using System.Collections.Generic;

namespace Mustache
{
    public struct Delimiter
    {
        public string Open;
        public string Close;

        public static Delimiter Default()
        {
            return new Delimiter { Open = "{{", Close = "}}" };
        }
    }

    public enum TokenType
    {
        Text,
        Variable,
        SectionOpen,
        SectionClose,
        InvertedSectionOpen,
        UnescapedVariable,
        Comment,
        Partial,
        DelimiterChange
    }

    public class Token
    {
        public string Template;
        public int StartIndex;
        public TokenType Type;
        public string Name;
        public string PartialIndent;
        public List<Token> Children;
        public bool IsBol;
        public int TextStartIndex;
        public int TextLength;
        public int SectionStartIndex;
        public int SectionEndIndex;
        public Delimiter CurrentDelimiter;

        public string Text
        {
            get
            {
                if (TextLength == 0)
                {
                    return string.Empty;
                }

                return Template.Substring(TextStartIndex, TextLength);
            }
        }

        public string SectionTemplate
        {
            get
            {
                return Template.Substring(SectionStartIndex, SectionEndIndex - SectionStartIndex + 1);
            }
        }
    }

    public class MustacheParser
    {
        public static TokenType TokenInfo(char c)
        {
            switch (c)
            {
                case '>': return TokenType.Partial;
                case '^': return TokenType.InvertedSectionOpen;
                case '/': return TokenType.SectionClose;
                case '&':
                case '{': return TokenType.UnescapedVariable;
                case '#': return TokenType.SectionOpen;
                case '!': return TokenType.Comment;
                case '=': return TokenType.DelimiterChange;
                default: return TokenType.Variable;
            }
        }

        static readonly HashSet<string> ProhibitedWords = new HashSet<string>()
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
            "char", "checked", "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
            "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
            "implicit", "in", "int", "interface", "internal", "is", "lock", "long",
            "namespace", "new", "null", "object", "operator", "out", "override",
            "params", "private", "protected", "public", "readonly", "ref", "return",
            "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
            "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
            "unchecked", "unsafe", "ushort", "using", "using static", "virtual", "void",
            "volatile", "while"
        };

        void SquashTokens(ref List<Token> tokens)
        {
            Func<string, bool> isWhiteText = (s) =>
            {
                foreach (var c in s)
                {
                    if (!(Char.IsWhiteSpace(c) || c == '\n' || c == '\r'))
                    {
                        return false;
                    }
                }
                return true;
            };

            uint tagMask = 0;
            var startIndex = 0;
            var removes = new List<int>();
            var tmp = new List<int>();
            var squashTokenMask = 1U << (int)TokenType.Comment |
                                  1U << (int)TokenType.SectionOpen |
                                  1U << (int)TokenType.InvertedSectionOpen |
                                  1U << (int)TokenType.SectionClose |
                                  1U << (int)TokenType.Partial |
                                  1U << (int)TokenType.DelimiterChange;

            for (int i = 0; i < tokens.Count; ++i)
            {
                tagMask |= 1U << (int)tokens[i].Type;

                if (i + 1 == tokens.Count || tokens[i + 1].IsBol)
                {
                    bool hasSquashToken = (tagMask & squashTokenMask) != 0;
                    bool textOnly = (tagMask & ~(squashTokenMask | (1U << (int)TokenType.Text))) == 0;

                    if (hasSquashToken && textOnly)
                    {
                        bool onlyWhiteText = true;
                        tmp.Clear();
                        for (int j = startIndex; j <= i; ++j)
                        {
                            if (tokens[j].Type == TokenType.Text)
                            {
                                if (!isWhiteText(tokens[j].Text))
                                {
                                    onlyWhiteText = false;
                                    break;
                                }
                                tmp.Add(j);
                            }
                            else if (tokens[j].Type == TokenType.Partial)
                            {
                                // NOTE: https://github.com/mustache/spec/blob/master/specs/partials.yml#L13-L15
                                var sb = new StringBuilder(tokens[j].PartialIndent);
                                foreach (var x in tmp)
                                {
                                    sb.Append(tokens[x].Text);
                                }
                                tokens[j].PartialIndent = sb.ToString();
                            }
                        }

                        if (onlyWhiteText)
                        {
                            removes.AddRange(tmp);
                        }
                    }

                    tagMask = 0;
                    startIndex = i + 1;
                }
            }

            for (int i = removes.Count - 1; 0 <= i; --i)
            {
                tokens.RemoveAt(removes[i]);
            }
        }
        List<Token> NestTokens(List<Token> tokens)
        {
            var root = new List<Token>();
            var collector = root;
            var sections = new List<Token>();

            foreach (var token in tokens)
            {
                if (token.Type == TokenType.SectionOpen || token.Type == TokenType.InvertedSectionOpen)
                {
                    token.Children = new List<Token>();
                    sections.Add(token);
                    collector.Add(token);
                    collector = token.Children;
                }
                else if (token.Type == TokenType.SectionClose)
                {
                    if (sections.Count == 0)
                    {
                        throw new MustacheException("Unopened section: " + token.Name);
                    }

                    var section = sections[sections.Count - 1];
                    sections.RemoveAt(sections.Count - 1);

                    if (section.Name != token.Name)
                    {
                        throw new MustacheException("Unclosed section: " + section.Name);
                    }

                    section.SectionEndIndex = token.StartIndex - 1;

                    if (0 < sections.Count)
                    {
                        collector = sections[sections.Count - 1].Children;
                    }
                    else
                    {
                        collector = root;
                    }
                }
                else
                {
                    collector.Add(token);
                }
            }

            if (0 < sections.Count)
            {
                var section = sections[sections.Count - 1];
                throw new MustacheException("Unclosed section: " + section.Name);
            }

            return root;
        }

        public List<Token> Parse(string template, Delimiter delimiter)
        {
            var scanner = new MustacheScanner(template);
            var tokens = new List<Token>();
            var start = scanner.Pos;
            var lastBol = 0;

            Action<int> pushText = (int end) =>
            {
                if (start < end && end <= template.Length)
                {
                    tokens.Add(new Token
                    {
                        Template = template,
                        Type = TokenType.Text,
                        TextStartIndex = start,
                        TextLength = end - start,
                        IsBol = start == lastBol,
                        CurrentDelimiter = delimiter,
                    });
                }
            };

            for (var ok = true; ok;)
            {
                if (scanner.Pos == 0 || scanner.Peek(-1) == '\n')
                {
                    lastBol = scanner.Pos;
                }

                if (scanner.Peek() == '\n')
                {
                    pushText(scanner.Pos + 1);
                    start = scanner.Pos + 1;
                    ok = scanner.Seek(1);
                }
                else if (scanner.StartsWith(delimiter.Open))
                {
                    if (start < scanner.Pos)
                    {
                        pushText(scanner.Pos);
                        start = scanner.Pos;
                    }

                    ok = scanner.Seek(delimiter.Open.Length); // discard begin tag

                    var tokenChar = scanner.Peek();
                    var tokenType = TokenInfo(tokenChar);
                    var value = string.Empty;

                    if (tokenType != TokenType.Variable)
                    {
                        scanner.Seek(1); // discard token type character
                    }

                    if (tokenType == TokenType.DelimiterChange)
                    {
                        value = scanner.ReadUntilJustBefore("=").Trim();
                        scanner.Seek(1); // discard '='
                        scanner.SeekUntilJustBefore(delimiter.Close);
                    }
                    else if (tokenType == TokenType.UnescapedVariable && tokenChar == '{')
                    {
                        value = scanner.ReadUntilJustBefore("}" + delimiter.Close).Trim();
                        scanner.Seek(1); // discard '}'
                    }
                    else
                    {
                        value = scanner.ReadUntilJustBefore(delimiter.Close).Trim();
                    }

                    if (!scanner.StartsWith(delimiter.Close))
                    {
                        throw new MustacheException("Unclosed tag at " + scanner.Pos);
                    }

                    if (tokenType != TokenType.Comment && tokenType != TokenType.DelimiterChange)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            throw new MustacheException("Empty token name at " + scanner.Pos);
                        }

                        foreach (var c in value)
                        {
                            if (!(char.IsLetterOrDigit(c) || c == '.' || c == '_'))
                            {
                                throw new MustacheException("Invalid token name '" + value + "' at " + scanner.Pos);
                            }
                        }

                        if (ProhibitedWords.Contains(value))
                        {
                            throw new MustacheException("Invalid token name '" + value + "' at " + scanner.Pos);
                        }
                    }

                    var token = new Token
                    {
                        Template = template,
                        Type = tokenType,
                        Name = value,
                        StartIndex = start,
                        IsBol = start == lastBol,
                        CurrentDelimiter = delimiter,
                    };

                    start = scanner.Pos + delimiter.Close.Length;
                    ok = scanner.Seek(delimiter.Close.Length); // discard end tag

                    token.SectionStartIndex = start;
                    tokens.Add(token);

                    if (tokenType == TokenType.DelimiterChange)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            throw new MustacheException("Empty delimiter at " + scanner.Pos);
                        }

                        var sp = value.Split(null as string[], StringSplitOptions.RemoveEmptyEntries);
                        if (sp.Length != 2)
                        {
                            throw new MustacheException("Invalid delimiter format at " + scanner.Pos);
                        }

                        delimiter = new Delimiter { Open = sp[0], Close = sp[1] };
                    }
                }
                else
                {
                    ok = scanner.Seek(1);
                }
            }

            pushText(scanner.Pos + 1);

            SquashTokens(ref tokens);
            return NestTokens(tokens);
        }

    }
}