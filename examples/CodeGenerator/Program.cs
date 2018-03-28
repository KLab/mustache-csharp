/*
    Demonstrates using the code generation utilities in conjunction with Mustache.
    All classes are kept in this file to make the example more readable.
 */


using CodeGenerationUtilities;
using Mustache;
using System;
using System.Linq;


namespace CodeGenerator
{
    /// <summary>
    /// Interface to generate implementation of
    /// </summary>
    internal interface INamed
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }
    }


    /// <summary>
    /// Hook to enable <see cref="INamed"/> code generation for a class
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class NameAttribute : Attribute
    {
        /// <summary>
        /// Name
        /// </summary>
        public readonly string Name;


        /// <summary>
        /// Initializes instance
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="name"/> invailed</exception>
        /// <param name="name">Name</param>
        public NameAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name invailed", "name");
            }


            Name = name;
        }
    }


    /// <summary>
    /// Partial (!) test class
    /// </summary>
    [Name("Jane Doe")]
    internal partial class MyNamed
    {
    }

    /// <summary>
    /// Another test class
    /// </summary>
    [Name("Jonn Doe")]
    internal partial class MyNamed2
    {
    }


    /// <summary>
    /// Example entry point
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Mustache string template for generating <see cref="INamed"/> implementation
        /// </summary>
        const string NamedTemplate =

@"namespace {{{Namespace}}}
{
    internal partial class {{{ClassName}}} : INamed
    {
        string INamed.Name
        {
            get { return ""{{{Name}}}""; }
        }
    }
}
";


        /// <summary>
        /// Convenience structure for holding <see cref="NamedTemplate"/> data
        /// </summary>
        private struct NamedData
        {
            /// <summary>
            /// Template 'Namespace' data
            /// </summary>
            public string Namespace { get; set; }

            /// <summary>
            /// Template 'ClassName' data
            /// </summary>
            public string ClassName { get; set; }

            /// <summary>
            /// Template 'Name' data
            /// </summary>
            public string Name { get; set; }
        }


        /// <summary>
        /// Runs example
        /// </summary>
        /// <param name="unused">Unused command line arguments</param>
        static void Main(string[] unused)
        {
            Console.WriteLine("Beginning code generation...");
            Console.WriteLine();
            Console.WriteLine();


            // Create template renderer
            var mustache = new MustacheRenderer();


            // Select all types with NameAttribute
            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.HasAttribute<NameAttribute>(false))
                .ToList()

                // For each type with NameAttribute
                // 1. prepare template data, and
                // 2. render it
                .ForEach(t =>
                {
                    var template = NamedTemplate;
                    var data     = new NamedData
                    {
                        Namespace = t.Namespace,
                        ClassName = t.Name,
                        Name      = t.GetAttribute<NameAttribute>(false).Name
                    };

                    var code = mustache.Render(template, data, null);

                    // Show generated code in console
                    Console.WriteLine(string.Format("'INamed' implementation for '{0}':", t.Name));
                    Console.WriteLine();
                    Console.WriteLine(code);
                    Console.WriteLine();
                });


            Console.WriteLine();
            Console.WriteLine("Code generation ran to completion...");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.Read();
        }
    }
}
