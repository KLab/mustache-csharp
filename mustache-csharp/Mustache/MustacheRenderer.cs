using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using Mustache.Extension;

namespace Mustache
{
    public class MustacheRenderer
    {
        public Dictionary<string, Func<MustacheContext, string>> Cache { get; private set; }
        public Dictionary<string, Func<MustacheContext, string>> PartialCache { get; private set; }
        public Dictionary<string, string> Partials { get; private set; }

        public MustacheRenderer()
        {
            Cache = new Dictionary<string, Func<MustacheContext, string>>();
            PartialCache = new Dictionary<string, Func<MustacheContext, string>>();
        }

        Func<MustacheContext, MustacheRenderer, string> CompileTokens(List<Token> tokens, string orginalTemplate)
        {
            var subs = new Dictionary<int, Func<MustacheContext, MustacheRenderer, string>>();
            Func<int, List<Token>, Func<MustacheContext, MustacheRenderer, string>> subRender = (subIndex, subTokens) =>
            {
                if (!subs.ContainsKey(subIndex))
                {
                    subs[subIndex] = CompileTokens(subTokens, orginalTemplate);
                }
                return subs[subIndex];
            };

            return (ctx, rnd) =>
            {
                var builder = new StringBuilder();

                for (int i = 0; i < tokens.Count; ++i)
                {
                    var token = tokens[i];
                    var next = string.Empty;
                    switch (token.Type)
                    {
                        case TokenType.SectionOpen:
                            next = rnd.RenderSection(token, ctx, subRender(i, token.Children), orginalTemplate);
                            break;
                        case TokenType.InvertedSectionOpen:
                            next = rnd.RenderInverted(token.Value, ctx, subRender(i, token.Children));
                            break;
                        case TokenType.Partial:
                            next = rnd.RenderPartial(token.Value, ctx, orginalTemplate);
                            break;
                        case TokenType.Variable:
                            next = rnd.RenderName(token.Value, ctx, true);
                            break;
                        case TokenType.UnescapedVariable:
                            next = rnd.RenderName(token.Value, ctx, false);
                            break;
                        case TokenType.Text:
                            next = token.Value;
                            break;
                    }
                    builder.Append(next);
                }

                return builder.ToString();
            };
        }

        Func<MustacheContext, string> Compile(string template, string[] tags, string orginalTemplate)
        {
            var tokens = new MustacheParser().Parse(template, tags);
            return Compile(tokens, orginalTemplate);
        }

        Func<MustacheContext, string> Compile(List<Token> tokens, string orginalTemplate)
        {
            var fn = CompileTokens(tokens, orginalTemplate);

            return (c) =>
            {
                return fn(c, this);
            };
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
                Cache[template] = Compile(template, null, template);
            }

            return Cache[template](new MustacheContext(view, null));
        }

        string RenderSection(Token token, MustacheContext ctx, Func<MustacheContext, MustacheRenderer, string> callback, string orgnalTemplate)
        {
            var value = ctx.Lookup(token.Value);

            if (value.IsFalsey())
            {
                return string.Empty;
            }

            if (value is ICollection)
            {
                var sb = new StringBuilder();
                foreach (object item in value as ICollection)
                {
                    sb.Append(callback(new MustacheContext(item, ctx), this));
                }
                return sb.ToString();
            }

            return callback(new MustacheContext(value, ctx), this);
        }


        string RenderInverted(string name, MustacheContext ctx, Func<MustacheContext, MustacheRenderer, string> callback)
        {
            var value = ctx.Lookup(name);

            if (value.IsFalsey())
            {
                return callback(ctx, this);
            }

            return string.Empty;
        }

        string RenderPartial(string name, MustacheContext ctx, string orgTemplate)
        {
            var fn = PartialCache.GetValueOrDefault(name);

            if (fn == null && Partials != null)
            {
                var partial = Partials.GetValueOrDefault(name);
                if (partial == null)
                {
                    return string.Empty;
                }

                fn = Compile(partial, null, orgTemplate);
                PartialCache[name] = fn;
            }

            if (fn == null)
            {
                return string.Empty;
            }

            return fn(ctx);
        }

        string RenderName(string name, MustacheContext ctx, bool escape)
        {
            var value = ctx.Lookup(name);

            var f = value as Func<MustacheContext, string>;
            if (f != null)
            {
                value = f(ctx);
            }

            if (value == null)
            {
                return string.Empty;
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