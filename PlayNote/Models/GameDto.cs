namespace PlayNote.Models
{
    /// <summary>
    /// Represents model for sharing <see cref="Entities.Game"/> entity. Also contains all the rating for the game.
    /// </summary>
    public class GameDto
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

        /// <summary>
        /// Total number of ratings for given game.
        /// </summary>
        public int NumberOfRatings
        {
            get
            {
                return Ratings.Count;
            }
        }

        /// <summary>
        /// List of ratings for given game.
        /// </summary>
        public ICollection<RatingDto> Ratings { get; set; } = new List<RatingDto>();
    }
}
