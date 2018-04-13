using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KLab.Mustache
{
    /// <summary>
    /// Mustache template renderer.
    /// </summary>
    public class MustacheRenderer : IDisposable
    {
        Dictionary<string, List<Token>> Cache { get; set; } = new Dictionary<string, List<Token>>();

        /// <summary>
        /// Partial templates.
        /// </summary>
        public Dictionary<string, string> Partials { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Enable strict mode.
        /// </summary>
        public bool StrictMode { get; set; } = false;

        /// <summary>
        /// Parses and applies given template and returns rendered string.
        /// </summary>
        /// <param name="template">Mustache template string.</param>
        /// <param name="data">Data object.</param>
        /// <param name="partials">Partial templates.</param>
        /// <returns>Rendered string.</returns>
        public string Render(string template, object data, Dictionary<string, string> partials = null)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

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
                Cache[template] = new MustacheParser().Parse(template, Delimiter.Default());
            }

            return RenderTokens(new MustacheContext(data, null), Cache[template]);
        }

        /// <summary>
        /// Removes all parsed tokens.
        /// </summary>
        public void ClearCache()
        {
            Cache.Clear();
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
                        builder.Append(RenderPartial(token, ctx, token.PartialIndent));
                        break;
                    case TokenType.Variable:
                        builder.Append(RenderName(token, ctx, true));
                        break;
                    case TokenType.UnescapedVariable:
                        builder.Append(RenderName(token, ctx, false));
                        break;
                    case TokenType.Text:
                        builder.Append(token.Template, token.StartIndex, token.TextLength);
                        break;
                }
            }

            return builder.ToString();
        }

        string RenderSection(Token token, MustacheContext ctx)
        {
            var value = ctx.Lookup(token.Name);

            if (value == null && StrictMode)
            {
                throw new MustacheException(string.Format("lookup failed name:'{0}' around:\n...{1}...\n", token.Name, token.SurroundingTemplate));
            }

            if (value.ShouldNotRender())
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
                        Cache[template] = new MustacheParser().Parse(template, token.CurrentDelimiter);
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

            if (value == null && StrictMode)
            {
                throw new MustacheException(string.Format("lookup failed name:'{0}' around:\n...{1}...\n", token.Name, token.SurroundingTemplate));
            }

            if (value.ShouldNotRender())
            {
                return RenderTokens(ctx, token.Children);
            }

            return string.Empty;
        }

        string RenderPartial(Token token, MustacheContext ctx, string indent)
        {
            string name = token.Name;

            if (Partials == null)
            {
                return string.Empty;
            }

            var partial = string.Empty;
            if (!Partials.TryGetValue(name, out partial))
            {
                return string.Empty;
            }

            var key = "# partial #" + name + "#" + indent;

            if (!Cache.ContainsKey(key))
            {
                if (string.IsNullOrEmpty(indent))
                {
                    Cache[key] = new MustacheParser().Parse(partial, Delimiter.Default());
                }
                else
                {
                    var replaced = Regex.Replace(partial, @"^(.+)$", indent + "$1", RegexOptions.Multiline);
                    Cache[key] = new MustacheParser().Parse(replaced, Delimiter.Default());
                }
            }

            return RenderTokens(ctx, Cache[key]);
        }

        string RenderName(Token token, MustacheContext ctx, bool escape)
        {
            var value = ctx.Lookup(token.Name);

            if (value == null && StrictMode)
            {
                throw new MustacheException(string.Format("lookup failed name:'{0}' around:\n...{1}...\n", token.Name, token.SurroundingTemplate));
            }

            if (value == null)
            {
                return string.Empty;
            }

            if (value.IsLambda())
            {
                var template = value.InvokeNameLambda() as string;
                var tokens = new MustacheParser().Parse(template, Delimiter.Default());
                value = RenderTokens(ctx, tokens);

                // Recheck for null
                if (value == null)
                {
                    return string.Empty;
                }
            }

            var s = value.ToString();

            if (s == "{ }")
            {
                return string.Empty;
            }

            if (escape)
            {
                return System.Web.HttpUtility.HtmlEncode(s);
            }

            return s;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Cache = null;
                    Partials = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
