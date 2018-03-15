using System;
using System.Collections.Generic;

namespace Mustache
{
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
        public int StartIndex;
        public int EndIndex;
        public int ClosingTagIndex;
        public TokenType Type;
        public string Value;
        public List<Token> Children;
    }

    public class MustacheParser
    {
        public static readonly string[] DefaultTags = { "{{", "}}" };

        public TokenType TokenInfo(char c)
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
        List<Token> SquashTokens(List<Token> tokens)
        {
            var txt = new List<string>();
            var res = new List<Token>();
            int txtStartIndex = 0;
            int txtEndIndex = 0;

            Action closeTxt = () =>
            {
                if (0 < txt.Count)
                {
                    res.Add(new Token
                    {
                        Type = TokenType.Text,
                        Value = string.Join("", txt),
                        StartIndex = txtStartIndex,
                        EndIndex = txtEndIndex
                    });
                    txt.Clear();
                }
            };

            foreach (var token in tokens)
            {
                if (token.Type == TokenType.Text)
                {
                    if (txt.Count == 0)
                    {
                        txtStartIndex = token.StartIndex;
                    }
                    txt.Add(token.Value);
                    txtEndIndex = token.EndIndex;
                }
                else
                {
                    closeTxt();
                    res.Add(token);
                }
            }
            closeTxt();

            return res;
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
                        // TODO: be better Exception
                        throw new Exception("Unopened section: " + token.Value);
                    }

                    var section = sections[sections.Count - 1];
                    sections.RemoveAt(sections.Count - 1);

                    if (section.Value != token.Value)
                    {
                        // TODO: be better Exception
                        throw new Exception("Unclosed section: " + section.Value);
                    }

                    section.ClosingTagIndex = token.StartIndex;

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
                // TODO: be better Exception
                var section = sections[sections.Count - 1];
                throw new Exception("Unclosed section: " + section.Value);
            }

            return root;
        }
        public List<Token> Parse(string template, string[] tags = null)
        {
            if (tags == null)
            {
                tags = DefaultTags;
            }

            var scanner = new MustacheScanner(template);
            var tokens = new List<Token>();
            var spaces = new List<int>();
            var hasTag = false;
            var nonSpace = false;

            Action stripSpace = () =>
            {
                if (hasTag && !nonSpace)
                {
                    for (int i = spaces.Count - 1; 0 <= i; --i)
                    {
                        tokens.RemoveAt(spaces[i]);
                    }
                }

                spaces.Clear();
                hasTag = false;
                nonSpace = false;
            };

            while (!scanner.Eos())
            {
                var start = scanner.Pos;
                var value = scanner.ReadUntilJustBefore(tags[0]);

                if (value != null)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (Char.IsWhiteSpace(value[i]))
                        {
                            spaces.Add(tokens.Count);
                        }
                        else
                        {
                            nonSpace = true;
                        }

                        tokens.Add(new Token
                        {
                            Type = TokenType.Text,
                            Value = value[i].ToString(),
                            StartIndex = start,
                            EndIndex = start,
                        });
                        start++;

                        if (value[i] == '\n')
                        {
                            stripSpace();
                        }
                    }
                }

                if (!scanner.StartsWith(tags[0]))
                {
                    break;
                }

                scanner.Seek(tags[0].Length); // discard right mustache

                hasTag = true;
                var tokenChar = scanner.Peek();
                var tokenType = TokenInfo(tokenChar);

                if (tokenType != TokenType.Variable)
                {
                    scanner.Seek(1);
                }

                if (tokenType == TokenType.DelimiterChange)
                {
                    value = scanner.ReadUntilJustBefore("=");
                    scanner.Seek(1); // discard left '='
                    scanner.SeekUntilJustBefore(tags[1]);
                }
                else if (tokenType == TokenType.UnescapedVariable && tokenChar == '{')
                {
                    value = scanner.ReadUntilJustBefore("}" + tags[1]);
                    scanner.Seek(1); // discard '}'
                }
                else
                {
                    value = scanner.ReadUntilJustBefore(tags[1]);
                }

                if (!scanner.StartsWith(tags[1]))
                {
                    // TODO: be better exception
                    throw new Exception("Unclosed tag at " + scanner.Pos);
                }

                scanner.Seek(tags[1].Length); // discard left mustache

                tokens.Add(new Token
                {
                    Type = tokenType,
                    Value = value.Trim(),
                    StartIndex = start,
                    EndIndex = scanner.Pos - 1,
                });

                if (tokenType == TokenType.Variable || tokenType == TokenType.UnescapedVariable)
                {
                    nonSpace = true; // --> what does this do?
                }

                if (tokenType == TokenType.DelimiterChange)
                {
                    tags = value.Split(null as string[], StringSplitOptions.RemoveEmptyEntries);
                }
            }

            return NestTokens(SquashTokens(tokens));
        }

    }
}