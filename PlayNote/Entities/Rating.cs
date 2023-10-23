using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayNote.Entities
{
    /// <summary>
    /// Represents rating for given game.
    /// </summary>
    public class Rating : IBaseEntity
    {
        /// <summary>
        /// Unique ID for the rating.
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Date and time in UTC when the rating was created. 
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Number of points for rating.
        /// </summary>
        [Required]
        public int Points { get; set; }

        /// <summary>
        /// Short optional review. 
        /// </summary>
        [MaxLength(1024)]
        public string? Review { get; set; }

        /// <summary>
        /// Navigation property for rated game.
        /// </summary>
        [ForeignKey(nameof(GameId))]
        public Game? Game { get; set; }

        /// <summary>
        /// Foreign key for rated game.
        /// </summary>
        public int GameId { get; set; }
    }
}
