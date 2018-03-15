using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Mustache
{
    static class Patterns
    {
        public static readonly string White = @"\s*";
        public static readonly string Space = @"\s+";
        public static readonly string Equal = @"\s*=";
        public static readonly string Curly = @"\s*}";
        public static readonly string Tag = @"[#\\^/>{&=!]";
    }

    public class Token
    {
        public int StartIndex;
        public int EndIndex;
        public int ClosingTagIndex;
        public string Type;
        public string Value;
        public List<Token> Children;
    }
    public class MustacheParser
    {
        public static readonly string[] DefaultTags = { "{{", "}}" };
        string[] EscapeTags(string[] tags)
        {
            return new string[] {
                Regex.Escape(tags[0]) + @"\s*",
                @"\s*" + Regex.Escape(tags[1]),
            };
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
                        Type = "text",
                        Value = string.Join("", txt),
                        StartIndex = txtStartIndex,
                        EndIndex = txtEndIndex
                    });
                    txt.Clear();
                }
            };

            foreach (var token in tokens)
            {
                if (token.Type == "text")
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
                if (token.Type == "#" || token.Type == "^")
                {
                    token.Children = new List<Token>();
                    sections.Add(token);
                    collector.Add(token);
                    collector = token.Children;
                }
                else if (token.Type == "/")
                {
                    if (sections.Count == 0)
                    {
                        // TODO: goo Exception
                        throw new Exception("Unopened section: " + token.Value);
                    }

                    var section = sections[sections.Count - 1];
                    sections.RemoveAt(sections.Count - 1);

                    if (section.Value != token.Value)
                    {
                        // TODO: goo Exception
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
                // TODO: goo Exception
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

            var tagPatterns = EscapeTags(tags);
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

            string type = string.Empty;
            string value = string.Empty;
            string chr = string.Empty;

            while (!scanner.Eos())
            {
                var start = scanner.Pos;
                value = scanner.ScanUntil(tagPatterns[0]);

                var spaceRe = new Regex(Patterns.Space);

                if (value != null)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        // FIXME: TOOOOO Slow

                        chr = value.Substring(i, 1);
                        var spaceMatch = spaceRe.Match(chr);

                        if (spaceMatch.Success)
                        {
                            spaces.Add(tokens.Count);
                        }
                        else
                        {
                            nonSpace = true;
                        }

                        tokens.Add(new Token
                        {
                            Type = "text",
                            Value = chr,
                            StartIndex = start,
                            EndIndex = start,
                        });
                        start++;

                        if (chr == "\n")
                        {
                            stripSpace();
                        }
                    }
                }

                if (string.IsNullOrEmpty(scanner.Scan(tagPatterns[0])))
                {
                    break;
                }

                hasTag = true;
                type = scanner.Scan(Patterns.Tag);

                if (string.IsNullOrEmpty(type))
                {
                    type = "name";
                }

                scanner.Scan(Patterns.White);

                if (type == "=")
                {
                    value = scanner.ScanUntil(Patterns.Equal);
                    scanner.Scan(Patterns.Equal);
                    scanner.ScanUntil(tagPatterns[1]);
                }
                else if (type == "{")
                {
                    var closePattern = @"\s*}" + Regex.Escape(tags[1]);
                    value = scanner.ScanUntil(closePattern);
                    scanner.Scan(Patterns.Curly);
                    scanner.ScanUntil(tagPatterns[1]);
                }
                else
                {
                    value = scanner.ScanUntil(tagPatterns[1]);
                }

                if (string.IsNullOrEmpty(scanner.Scan(tagPatterns[1])))
                {
                    // TODO good exception type
                    throw new Exception("Unclosed tag at " + scanner.Pos);
                }

                tokens.Add(new Token
                {
                    Type = type,
                    Value = value,
                    StartIndex = start,
                    EndIndex = scanner.Pos - 1,
                });

                if (type == "name" || type == "{" || type == "&")
                {
                    nonSpace = true; // --> what does this do?
                }

                if (type == "=")
                {
                    tags = value.Split(null as string[], StringSplitOptions.RemoveEmptyEntries);
                    tagPatterns = EscapeTags(tags);
                }
            }

            return NestTokens(SquashTokens(tokens));
        }

    }
}