using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mustache.Extension;

namespace Mustache
{
    public class MustacheRenderer
    {
        public Dictionary<string, List<Token>> Cache { get; private set; }
        public Dictionary<string, string> Partials { get; private set; }

        public Delimiter CurrentDelimiter { get; private set; }

        public MustacheRenderer()
        {
            Cache = new Dictionary<string, List<Token>>();
            CurrentDelimiter = Delimiter.Default();
        }

        public string Render(string template, object view, Dictionary<string, string> partials)
        {
            if (partials != null)
            {
                Partials = partials;
            }

            if (string.IsNullOrEmpty(template))
            {
                return string.Empty;
            }

            if (!Cache.ContainsKey(template))
            {
                var parsed = new MustacheParser().Parse(template, Delimiter.Default());
                CurrentDelimiter = parsed.Item2;
                Cache[template] = parsed.Item1;
            }

            return RenderTokens(new MustacheContext(view, null), Cache[template]);
        }


        string RenderTokens(MustacheContext ctx, List<Token> tokens)
        {
            var builder = new StringBuilder();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.SectionOpen:
                        builder.Append(RenderSection(token, ctx));

                        break;
                    case TokenType.InvertedSectionOpen:
                        builder.Append(RenderInverted(token, ctx));
                        break;
                    case TokenType.Partial:
                        builder.Append(RenderPartial(token.Name, ctx, token.PartialIndent));
                        break;
                    case TokenType.Variable:
                        builder.Append(RenderName(token.Name, ctx, true));
                        break;
                    case TokenType.UnescapedVariable:
                        builder.Append(RenderName(token.Name, ctx, false));
                        break;
                    case TokenType.Text:
                        builder.Append(token.Template, token.TextStartIndex, token.TextLength);
                        break;
                }
            }

            return builder.ToString();
        }
        string RenderSection(Token token, MustacheContext ctx)
        {
            var value = ctx.Lookup(token.Name);

            if (value.IsFalsey())
            {
                return string.Empty;
            }

            if (value.IsLambda())
            {
                var template = value.InvokeSectionLambda(token.SectionTemplate) as string;
                if (!string.IsNullOrEmpty(template))
                {
                    if (!Cache.ContainsKey(template))
                    {
                        // When Lambdas are used as the data value for Section tag,
                        // the returned value MUST be rendered against the current delimiters.
                        var parsed = new MustacheParser().Parse(template, CurrentDelimiter);
                        Cache[template] = parsed.Item1;
                    }
                    return RenderTokens(ctx, Cache[template]);
                }
            }

            if (value is ICollection)
            {
                var sb = new StringBuilder();
                foreach (object item in value as ICollection)
                {
                    sb.Append(RenderTokens(new MustacheContext(item, ctx), token.Children));
                }
                return sb.ToString();
            }

            return RenderTokens(new MustacheContext(value, ctx), token.Children);
        }


        string RenderInverted(Token token, MustacheContext ctx)
        {
            var value = ctx.Lookup(token.Name);

            if (value.IsFalsey())
            {
                return RenderTokens(ctx, token.Children);
            }

            return string.Empty;
        }

        string RenderPartial(string name, MustacheContext ctx, string indent)
        {
            if (Partials == null)
            {
                return string.Empty;
            }

            var partial = Partials.GetValueOrDefault(name);
            if (partial == null)
            {
                return string.Empty;
            }

            var key = "#partial" + name + "#" + indent;

            if (!Cache.ContainsKey(key))
            {
                if (string.IsNullOrEmpty(indent))
                {
                    var parsed = new MustacheParser().Parse(partial, Delimiter.Default());
                    Cache[key] = parsed.Item1;
                }
                else
                {
                    // FIXME: dirty
                    var replaced = string.Empty;
                    if (partial[partial.Length - 1] != '\n')
                    {
                        replaced = Regex.Replace(partial, @"^", indent, RegexOptions.Multiline);
                    }
                    else
                    {
                        var s = partial.Substring(0, partial.Length - 1);
                        replaced = Regex.Replace(s, @"^", indent, RegexOptions.Multiline) + "\n";
                    }

                    var parsed = new MustacheParser().Parse(replaced, Delimiter.Default());
                    Cache[key] = parsed.Item1;
                }
            }

            return RenderTokens(ctx, Cache[key]);
        }

        string RenderName(string name, MustacheContext ctx, bool escape)
        {
            var value = ctx.Lookup(name);

            if (value == null)
            {
                return string.Empty;
            }

            if (value.IsLambda())
            {
                var tpl = value.InvokeNameLambda() as string;
                var parsed = new MustacheParser().Parse(tpl, Delimiter.Default());
                value = RenderTokens(ctx, parsed.Item1);
            }

            if (value.IsFalsey())
            {
                return string.Empty;
            }

            if (escape)
            {
                return System.Web.HttpUtility.HtmlEncode(value.ToString());
            }

            return value.ToString();
        }
    }
}