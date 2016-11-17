using System;

namespace Common.Tests.Startup
{
    /// <summary>
    /// Attribute for annotating startup methods with 3 arguments:
    /// - <see cref="string"/>
    /// - <see cref="int"/>
    /// - <see cref="IComplexUserType"/>
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ComplexStartupAttribute : Attribute
    {
    }
}
