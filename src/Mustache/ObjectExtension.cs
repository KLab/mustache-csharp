using System;
using System.Reflection;
using System.Collections;

namespace Mustache
{
    /// <summary>
    /// Object type extensions
    /// </summary>
    static class ObjectExtension
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
            if (f == null)
            {
                return null;
            }
            return f.GetValue(self);
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
            if (p == null)
            {
                return null;
            }
            return p.GetValue(self);
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
            if (f != null)
            {
                return f;
            }

            var p = self.GetPropertyValue(name);
            if (p != null)
            {
                return p;
            }

            return null;
        }

        /// <summary>
        /// Invoke this delegate without any arguments
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
        /// Invoke this delegate with template argument
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
            return
                self is Func<string, string> ||
                self is Func<string, bool> ||
                self is Func<string> ||
                self is Func<bool>;
        }

        /// <summary>
        /// Check this object is a false like value
        /// </summary>
        /// <param name="self">object instance</param>
        /// <returns>true if this is a false like value</returns>
        public static bool ShouldNotRender(this object self)
        {
            if (self == null)
            {
                return true;
            }
            if (self is IMustacheRenderFilter)
            {
                return !(self as IMustacheRenderFilter).ShouldRender;
            }
            if (self is bool)
            {
                return !(bool)self;
            }
            if (self is string)
            {
                return string.IsNullOrEmpty(self as String);
            }
            if (self is IConvertible)
            {
                return !Convert.ToBoolean(self as IConvertible);
            }
            if (self is IEnumerable)
            {
                return !(self as IEnumerable).GetEnumerator().MoveNext();
            }
            // INV { < How to detect anonymous empty value smarter? 
            if (self.ToString() == "{ }")
            {
                return true;
            }

            return false;
        }
    }
}
