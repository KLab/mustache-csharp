using System;
using System.Collections.Generic;
using Mustache.Extension;

namespace Mustache
{
    /// <summary>
    /// Mustache Context
    /// </summary>
    public class MustacheContext
    {
        object Data;
        MustacheContext Parent;
        Dictionary<string, object> Cache;

        /// <summary>
        /// Initialize context
        /// </summary>
        /// <param name="data">the data associated with this context</param>
        /// <param name="parent">parent context</param>
        public MustacheContext(object data, MustacheContext parent)
        {
            Data = data;
            Parent = parent;
            Cache = new Dictionary<string, object>();
        }

        /// <summary>
        /// Find the name key in the current context.
        /// If there is no name key, the parent contexts will be checked recursively.
        /// </summary>
        /// <param name="name">the name key</param>
        /// <returns>associated data</returns>
        public object Lookup(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Trim() == string.Empty)
            {
                throw new ArgumentException("empty name", "name");
            }

            if (Cache.ContainsKey(name))
            {
                return Cache[name];
            }

            if (Data == null)
            {
                return null;
            }

            object value = null;

            if (name == ".")
            {
                value = Data;
            }
            else
            {
                var ctx = this;

                while (ctx != null)
                {
                    if (name.Contains("."))
                    {
                        value = ctx.Data;

                        foreach (var s in name.Split('.'))
                        {
                            var v = value.GetValue(s);
                            if (v != null)
                            {
                                value = v;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        var v = ctx.Data.GetValue(name);
                        if (v != null)
                        {
                            value = v;
                        }
                    }

                    if (value != null)
                    {
                        break;
                    }

                    ctx = ctx.Parent;
                }
            }

            return value;
        }
    }
}
