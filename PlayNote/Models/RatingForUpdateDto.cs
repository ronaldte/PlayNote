namespace PlayNote.Models
{
    /// <summary>
    /// Represents model for updating new <see cref="Entities.Rating"/> entity.
    /// </summary>
    public class RatingForUpdateDto
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
