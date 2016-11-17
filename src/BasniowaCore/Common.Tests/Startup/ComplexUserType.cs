namespace Common.Tests.Startup
{
    /// <summary>
    /// Sample testing type.
    /// </summary>
    /// <seealso cref="Common.Tests.Startup.IComplexUserType" />
    public class ComplexUserType : IComplexUserType
    {
        /// <inheritdoc/>
        public int Id { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }
    }
}