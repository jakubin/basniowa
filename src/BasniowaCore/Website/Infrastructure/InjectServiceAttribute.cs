using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Website.Infrastructure
{
    /// <summary>
    /// Specifies that a controller property should be bound using the request services.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InjectServiceAttribute : Attribute, IBindingSourceMetadata
    {
        /// <inheritdoc />
        public BindingSource BindingSource => BindingSource.Services;
    }
}
