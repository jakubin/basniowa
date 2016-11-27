using System;

namespace Common.CodeAnalysis
{
    /// <summary>
    /// Excludes a file from code coverage.
    /// This is a temporary hack as .NET Core does not ship this attribute yet.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, Inherited = false, AllowMultiple = false)]
    public sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeFromCodeCoverageAttribute"/> class.
        /// </summary>
        public ExcludeFromCodeCoverageAttribute()
        {
        }
    }
}
