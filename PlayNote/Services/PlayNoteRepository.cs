using Microsoft.EntityFrameworkCore;
using PlayNote.DbContexts;
using PlayNote.Entities;
using System.Linq.Expressions;

namespace PlayNote.Services
{
    /// <summary>
    /// Implementation of repository layer for <see cref="Game"/> and <see cref="Rating"/> entities.
    /// </summary>
    public class PlayNoteRepository : IPlayNoteRepository
    {
        private readonly PlayNoteDbContext _context;
        
        /// <summary>
        /// Creates new instance of <see cref="PlayNoteRepository"/>.
        /// </summary>
        /// <param name="context"><see cref="PlayNoteDbContext"/> used as storage.</param>
        public PlayNoteRepository(PlayNoteDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public Task<bool> GameExistsAsync(int gameId)
        {
            return _context.Games.AnyAsync(g => g.Id == gameId);
        }

        /// <inheritdoc/>
        private async Task<(IEnumerable<T>, PaginationMetadata)> GetAllAsync<T>
            (IQueryable<T> entities, Expression<Func<T, bool>> filter, string? searchQuery, int pageNumber, int pageSize) where T : class, IBaseEntity
        {
            if (!string.IsNullOrEmpty(searchQuery))
            {
                entities = entities.Where(filter);
            }

            var entitiesTotalCount = await entities.CountAsync();
            var paginationMetadata = new PaginationMetadata(entitiesTotalCount, pageSize, pageNumber);

            var entitiesToReturn = await entities.Paginate(pageNumber, pageSize).ToListAsync();

            return (entitiesToReturn, paginationMetadata);
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<Game>, PaginationMetadata)> GetAllGamesAsync(string? searchQuery, int pageNumber, int pageSize)
        {
            return await GetAllAsync<Game>(
                _context.Games, g => g.Name.Contains(searchQuery!) || (g.Description != null && g.Description.Contains(searchQuery!)),
                searchQuery, pageNumber, pageSize );
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<Rating>, PaginationMetadata)> GetAllRatingsForGameAsync(int gameId, string? searchQuery, int pageNumber, int pageSize)
        { 
            return await GetAllAsync<Rating>(
                _context.Ratings.Where(r => r.GameId == gameId), r => r.Review != null && r.Review.Contains(searchQuery!), searchQuery,
                pageNumber, pageSize );
        }

        /// <inheritdoc/>
        public async Task<Game?> GetGameAsync(int gameId, bool includeRatings)
        {
            var games = _context.Games as IQueryable<Game>;

            games = games.Where(g => g.Id == gameId);

            if (includeRatings)
            {
                games = games.Include(g => g.Ratings);
            }

            return await games.FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<Rating?> GetRatingForGameAsync(int gameId, int ratingId)
        {
            return await _context.Ratings.Where(r => r.GameId == gameId && r.Id == ratingId).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public void DeleteRating(Rating rating)
        {
            _context.Ratings.Remove(rating);
        }

        /// <inheritdoc/>
        public async Task<bool> SaveChangesAsync()
        {
            return (await  _context.SaveChangesAsync() >= 0);
        }

        /// <inheritdoc/>
        public async Task AddRatingAsync(int gameId, Rating rating)
        {
            var game = await GetGameAsync(gameId, false);
            if (game != null)
            {
                game.Ratings.Add(rating);
            }
        }
    }
}
