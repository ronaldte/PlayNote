namespace PlayNote.Entities
{
    /// <summary>
    /// Represents base entity with properties applied for all entites.
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// Unique identifier for an entity.
        /// </summary>
        public int Id { get; set; } 
    }
}
