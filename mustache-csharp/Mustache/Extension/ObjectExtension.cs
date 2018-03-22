using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustache.Extension
{
    /// <summary>
    /// Object type extensions
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// get field value by reflection
        /// </summary>
        /// <param name="self">object instance</param>
        /// <param name="name">field name</param>
        /// <returns>field value</returns>
        public static object GetFieldValue(this object self, string name)
        {
            var f = self.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (f == null) ? null : f.GetValue(self);
        }

        /// <summary>
        /// get property value by reflection
        /// </summary>
        /// <param name="self">object instance</param>
        /// <param name="name">property name</param>
        /// <returns>property value</returns>
        public static object GetPropertyValue(this object self, string name)
        {
            var p = self.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (p == null) ? null : p.GetValue(self);
        }

        /// <summary>
        /// get field or property value
        /// </summary>
        /// <param name="self">object instance</param>
        /// <param name="name">name</param>
        /// <returns>the value</returns>
        public static object GetValue(this object self, string name)
        {
            var f = self.GetFieldValue(name);
            if (f != null) return f;

            var p = self.GetPropertyValue(name);
            if (p != null) return p;

            return null;
        }

        /// <summary>
        /// Invoke this delegate as specified types
        /// </summary>
        /// <param name="self">object instance</param>
        /// <returns>the value this delegate returned</returns>
        public static object InvokeNameLambda(this object self)
        {
            if (self is Func<string>) return (self as Func<string>)();
            if (self is Func<bool>) return (self as Func<bool>)();
            return null;
        }

        /// <summary>
        /// Invoke this delegate as specified types
        /// </summary>
        /// <param name="self">object instance</param>
        /// <param name="template">string to pass lambda</param>
        /// <returns>the value this delegate returned</returns>
        public static object InvokeSectionLambda(this object self, string template)
        {
            if (self is Func<string, string>) return (self as Func<string, string>)(template);
            if (self is Func<string, bool>) return (self as Func<string, bool>)(template);
            return null;
        }

        /// <summary>
        /// Check this object is a supported lambda types
        /// </summary>
        /// <param name="self">object instance</param>
        /// <returns>true if this is a supported lambda type otherwise false</returns>
        public static bool IsLambda(this object self)
        {
            if (self is Func<string, string>) return true;
            if (self is Func<string, bool>) return true;
            if (self is Func<string>) return true;
            if (self is Func<bool>) return true;
            return false;
        }

        /// <summary>
        /// Check this object is a false like value
        /// </summary>
        /// <param name="self">object instance</param>
        /// <returns>true if this is a false like value</returns>
        public static bool IsFalsey(this object self)
        {
            if (self == null) return true;
            if (self is int || self is long || self is float || self is double) return false;
            if (self is bool) return !(bool)self;
            if (self is string) return (self as string).Length == 0;
            if (self is ICollection) return (self as ICollection).Count == 0;
            if (self is IEnumerable) return !(self as IEnumerable).GetEnumerator().MoveNext();
            // FIXME:{ < how to detect anonymous empty value more smartly. 
            if (self.ToString() == "{ }") return true;
            return false;
        }
    }
}