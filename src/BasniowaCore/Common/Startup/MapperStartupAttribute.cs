using System;

namespace Common.Startup
{
    /// <summary>
    /// Attribute for annotating methods with set up logic for AutoMapper configurations.
    /// Annotated methods must have signature (method and argument names are ignored):
    /// <code>
    /// public static void ConfigureMapper(IMapperConfigurationExpression cfg)
    /// </code>
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public class MapperStartupAttribute : Attribute
    {
    }
}
