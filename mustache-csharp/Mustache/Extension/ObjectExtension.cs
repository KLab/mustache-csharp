using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustache.Extension
{
    public static class ObjectExtension
    {
        public static object GetFieldValue(this object self, string name)
        {
            var f = self.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (f == null) ? null : f.GetValue(self);
        }

        public static object GetPropertyValue(this object self, string name)
        {
            var p = self.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (p == null) ? null : p.GetValue(self);
        }

        public static object GetValue(this object self, string name)
        {
            var f = self.GetFieldValue(name);
            if (f != null) return f;

            var p = self.GetPropertyValue(name);
            if (p != null) return p;

            return null;
        }

        public static bool IsFalsey(this object self)
        {
            if (self == null)
            {
                return true;
            }

            if (self is int || self is long || self is float || self is double)
            {
                return false;
            }

            if (self is bool)
            {
                return !(bool)self;
            }

            if (self is string)
            {
                return (self as string).Length == 0;
            }

            if (self is ICollection)
            {
                return (self as ICollection).Count == 0;
            }

            if (self is IEnumerable)
            {
                return !(self as IEnumerable).GetEnumerator().MoveNext();
            }

            if (self.ToString() == "{ }")
            {
                // FIXME:{ < how to detect anonymous empty value more smartly. 
                return true;
            }

            return false;
        }
    }



}
