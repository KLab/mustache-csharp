using System;
using System.Collections.Generic;
using Mustache.Extension;

namespace Mustache
{
    public class MustacheContext
    {
        public object View;
        public MustacheContext Parent;
        public Dictionary<string, object> Cache;

        public MustacheContext()
        {
            View = null;
            Parent = null;
            Cache = new Dictionary<string, object>();
        }

        public MustacheContext(object view, MustacheContext parent)
        {
            View = view;
            Parent = parent;
            Cache = new Dictionary<string, object>();
        }

        public object Lookup(string name)
        {
            if (Cache.ContainsKey(name))
            {
                return Cache[name];
            }

            if (View == null)
            {
                return null;
            }

            object value = null;

            if (name == ".")
            {
                value = View;
            }
            else
            {
                var ctx = this;

                while (ctx != null)
                {
                    if (0 < name.IndexOf('.'))
                    {
                        var names = name.Split(new char[] { '.' });
                        value = ctx.View;

                        foreach (var s in names)
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
                        var v = ctx.View.GetValue(name);
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
