using System;

namespace KLab.Mustache
{
    /// <summary>
    /// MustacheContext holds the data associated with template.
    /// </summary>
    public class MustacheContext
    {
        object Data { get; set; }
        MustacheContext Parent { get; set; }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="data">The data to be associated with this context.</param>
        /// <param name="parent">Parent context.</param>
        public MustacheContext(object data, MustacheContext parent)
        {
            Data = data;
            Parent = parent;
        }

        /// <summary>
        /// Finds the name key in the current context and returns the value.
        /// If there is no name key, the parent contexts will be checked recursively.
        /// </summary>
        /// <param name="name">The name key.</param>
        /// <returns>Found data.</returns>
        public object Lookup(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Trim() == string.Empty)
            {
                throw new ArgumentException("Empty name", "name");
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
