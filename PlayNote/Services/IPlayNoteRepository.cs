using PlayNote.Entities;

namespace PlayNote.Services
{
    /// <summary>
    /// Represents repository layer for <see cref="Game"/> and <see cref="Rating"/> entities.
    /// </summary>
    public interface IPlayNoteRepository
    {
        /// <summary>
        /// Returns game with given id.
        /// </summary>
        /// <param name="gameId">Id of the game to return.</param>
        /// <param name="includeRatings">If the game should include all ratings.</param>
        /// <returns><see cref="Game"/>entity from storage.</returns>
        Task<Game?> GetGameAsync(int gameId, bool includeRatings);
        
        /// <summary>
        /// Returns list of all games containing search query and pagination metadata.
        /// </summary>
        /// <param name="searchQuery">Search string to be contained in the games.</param>
        /// <param name="pageNumber">Number of the page to return.</param>
        /// <param name="pageSize">Size of the page to be used in pagination.</param>
        /// <returns></returns>
        Task<(IEnumerable<Game>, PaginationMetadata)> GetAllGamesAsync(string? searchQuery, int pageNumber, int pageSize);
        
        /// <summary>
        /// Checks if game exists.
        /// </summary>
        /// <param name="gameId">Id of the game to be checked.</param>
        /// <returns>Boolean if the game exists in storage.</returns>
        Task<bool> GameExistsAsync(int gameId);

        /// <summary>
        /// Returns a list with all ratings for given game, containing search string and pagination metadata.
        /// </summary>
        /// <param name="gameId">Id of the game with ratings to be returned.</param>
        /// <param name="searchQuery">String query to be contained in the ratings.</param>
        /// <param name="pageNumber">Number of the page to be returned.</param>
        /// <param name="pageSize">Size of the page to be used in pagination.</param>
        /// <returns></returns>
        Task<(IEnumerable<Rating>, PaginationMetadata)> GetAllRatingsForGameAsync(int gameId, string? searchQuery, int pageNumber, int pageSize);
        
        /// <summary>
        /// Return a single entity of rating for given game.
        /// </summary>
        /// <param name="gameId">Id of the game for which the rating is.</param>
        /// <param name="ratingId">Id of the rating to be returned.</param>
        /// <returns>Single instance of rating or null if not found.</returns>
        Task<Rating?> GetRatingForGameAsync(int gameId, int ratingId);
        
        /// <summary>
        /// Created new entity for given game.
        /// </summary>
        /// <param name="gameId">Id of the game for which the rating is.</param>
        /// <param name="rating">New rating entity to be created.</param>
        Task AddRatingAsync(int gameId, Rating rating);        
        
        /// <summary>
        /// Deleted given rating entity from storage.
        /// </summary>
        /// <param name="rating">Rating to be deleted.</param>
        void DeleteRating(Rating rating);

        /// <summary>
        /// Saves changes in storage.
        /// </summary>
        /// <returns>Boolean if any changes were saved.</returns>
        Task<bool> SaveChangesAsync();
    }
}
