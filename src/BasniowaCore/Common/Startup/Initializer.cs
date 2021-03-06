﻿using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Common.Startup
{
    /// <summary>
    /// Runs initializing logic distributed through the solution.
    /// </summary>
    public static class Initializer
    {
        /// <summary>
        /// Runs initializer methods annotated with attribute <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Attribute used to indicate methods to run.</typeparam>
        /// <param name="arguments">The arguments to pass to methods.</param>
        public static void Init<T>(params object[] arguments)
            where T : Attribute
        {
            var attributeType = typeof(T);

            var name = new AssemblyName(attributeType.GetTypeInfo().Assembly.FullName).Name;

            var referencingLibraries = DependencyContext.Default.CompileLibraries
                .Where(x =>
                    string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)
                    || x.Dependencies.Any(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var library in referencingLibraries)
            {
                var assembly = Assembly.Load(new AssemblyName(library.Name));
                var types = assembly.GetExportedTypes();
                foreach (var type in types)
                {
                    var allMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                    var startupMethods = allMethods
                        .Where(x => x.GetCustomAttributes<T>().Any())
                        .ToArray();

                    foreach (var startupMethod in startupMethods)
                    {
                        startupMethod.Invoke(null, arguments);
                    }
                }
            }
        }
    }
}
