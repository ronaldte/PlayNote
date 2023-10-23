namespace PlayNote.Models
{
    /// <summary>
    /// Represents model for sharing <see cref="Entities.Game"/> entity.
    /// </summary>
    public class GameWithoutRatingsDto
    {
        /// <summary>
        /// Identifier for game.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the game.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Description of the game.
        /// </summary>
        public string? Description { get; set; }
    }
}
