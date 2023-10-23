namespace PlayNote.Models
{
    /// <summary>
    /// Represents model for creating new <see cref="Entities.Rating"/> entity.
    /// </summary>
    public class RatingForCreationDto
    {
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
