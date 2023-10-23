namespace PlayNote.Models
{
    /// <summary>
    /// Represents model for sharing <see cref="Entities.Rating"/> entity.
    /// </summary>
    public class RatingDto
    {
        /// <summary>
        /// Identifier for the rating.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// UTC date and time of when the rating was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Number of points given in the rating.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Review for the game.
        /// </summary>
        public string? Review { get; set; }
    }
}
