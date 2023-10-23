using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayNote.Entities
{
    /// <summary>
    /// Represents the game.
    /// </summary>
    public class Game : IBaseEntity
    {
        /// <summary>
        /// Unique ID for game.
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Name of the game.
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Optional description of the game.
        /// </summary>
        [MaxLength(1024)]
        public string? Description { get; set; }

        /// <summary>
        /// List of all ratings for the game.
        /// </summary>
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
