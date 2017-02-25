namespace Website.Infrastructure.ErrorHandling
{
    /// <summary>
    /// Model containing details of entity, which was not found.
    /// </summary>
    /// <remarks>
    /// It is set in <see cref="ErrorModel.Details"/> fields.
    /// </remarks>
    public class EntityNotFoundModel
    {
        /// <summary>
        /// The type of the entity.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// The entity key.
        /// </summary>
        public string EntityKey { get; set; }
    }
}