using System;

namespace Mustache
{
    /// <summary>
    /// Mustache Context
    /// </summary>
    public class MustacheContext
    {
        object Data { get; set; }
        MustacheContext Parent { get; set; }

        /// <summary>
        /// Initialize context
        /// </summary>
        /// <param name="data">the data associated with this context</param>
        /// <param name="parent">parent context</param>
        public MustacheContext(object data, MustacheContext parent)
        {
            Data = data;
            Parent = parent;
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
                            value = value.GetValue(s);
                            if (value == null)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        value = ctx.Data.GetValue(name);
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
