using System;

namespace Common.Tests.Startup
{
    /// <summary>
    /// Attribute for annotating startup methods without argument.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class MultipleStartupAttribute : Attribute
    {
    }
}
