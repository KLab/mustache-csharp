/*
    Provides System.Type extension methods convenient for code generation.
 */


using System;
using System.Linq;


namespace CodeGenerationUtilities
{
    /// <summary>
    /// <see cref="Type"/> extension methods
    /// </summary>
    public static class TypeExtensionMethods
    {
        /// <summary>
        /// Gets attribute by type
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="self">Instance</param>
        /// <param name="inherit">Inheritance control</param>
        /// <exception cref="NullReferenceException"><paramref name="self"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException"><paramref name="self"/> doesn't have <typeparamref name="T"/></exception>
        /// <returns>Attribute instance</returns>
        public static T GetAttribute<T>(this Type self, bool inherit) where T : Attribute
        {
            if (self == null)
            {
                throw new NullReferenceException();
            }

            if (!self.HasAttribute<T>(inherit))
            {
                throw new InvalidOperationException(string.Format("{0} doesn't have attribute of type {1}", self.Name, typeof(T).Name));
            }


            return self
                .GetCustomAttributes(typeof(T), inherit)
                .FirstOrDefault() as T;
        }


        /// <summary>
        /// Checks whether type has attribute
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="self">Instance</param>
        /// <param name="inherit">Inheritance control</param>
        /// <returns><see langword="true"/> if <paramref name="self"/> isn't <see langword="null"/> and has attribute; <see langword="false"/> otherwise</returns>
        public static bool HasAttribute<T>(this Type self, bool inherit) where T : Attribute
        {
            if (self == null)
            {
                return false;
            }


            return self
                .GetCustomAttributes(typeof(T), inherit)
                .FirstOrDefault() != null;
        }
    }
}
