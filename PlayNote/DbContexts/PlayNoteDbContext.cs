using Microsoft.EntityFrameworkCore;
using PlayNote.Entities;

namespace PlayNote.DbContexts
{
    /// <summary>
    /// Default database context for accessing DB.
    /// </summary>
    public class PlayNoteDbContext : DbContext
    {
        /// <summary>
        /// Accessing table for <see cref="Game"/> entities.
        /// </summary>
        public DbSet<Game> Games { get; set; } = null!;
        
        /// <summary>
        /// Accessing table for <see cref="Rating"/> entities.
        /// </summary>
        public DbSet<Rating> Ratings { get; set; } = null!;

        /// <summary>
        /// Initialized new <see cref="PlayNoteDbContext"/> with provided options.
        /// </summary>
        /// <param name="options"></param>
        public PlayNoteDbContext(DbContextOptions<PlayNoteDbContext> options) : base(options) 
        { 
            
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rating>()
                .Property(r => r.CreatedAtUtc)
                .HasDefaultValueSql("datetime('now')");

        }
    }
}
