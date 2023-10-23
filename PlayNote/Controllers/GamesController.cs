using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlayNote.Models;
using PlayNote.Services;
using System.Text.Json;

namespace PlayNote.Controllers
{
    /// <summary>
    /// Routes for controlling games.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private const int maxGamesPageSize = 20;

        private readonly IPlayNoteRepository _playNoteRpository;
        private readonly IMapper _mapper;
        private readonly ILogger<GamesController> _logger;

        /// <summary>
        /// Creates instance for games controller.
        /// </summary>
        /// <param name="playNoteRpository">Repository for working with storage.</param>
        /// <param name="mapper">Mapper for mapping entities to dtos.</param>
        /// <param name="logger">Logger for logging events.</param>
        /// <exception cref="ArgumentNullException">If any of the required services are missing.</exception>
        public GamesController(IPlayNoteRepository playNoteRpository, IMapper mapper, ILogger<GamesController> logger)
        {
            _playNoteRpository = playNoteRpository ?? throw new ArgumentNullException(nameof(playNoteRpository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns game based on given Id.
        /// </summary>
        /// <param name="gameId">Id of the game ot be returned.</param>
        /// <param name="includeRatings">Also returns list of all ratings for given game.</param>
        /// <returns><see cref="GameDto"/> for found game with all ratings or <see cref="GameWithoutRatingsDto"/> for just the game.</returns>
        /// <response code="200">Returns the requested game.</response>
        /// <response code="404">Game with given id was not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{gameId:int}")]
        public async Task<IActionResult> GetGame(int gameId, bool includeRatings = false)
        {
            var gameEntity = await _playNoteRpository.GetGameAsync(gameId, includeRatings);
            if (gameEntity == null)
            {
                _logger.LogInformation("Game entity with id: {gameId} not found", gameId);
                return NotFound($"Game with Id {gameId} does not exist.");
            }

            if (includeRatings)
            {
                return Ok(_mapper.Map<GameDto>(gameEntity));
            }

            return Ok(_mapper.Map<GameWithoutRatingsDto>(gameEntity));
        }

        /// <summary>
        /// Returns all games.
        /// </summary>
        /// <param name="searchQuery">String to be contained in the games.</param>
        /// <param name="pageNumber">Number of the page to be loaded.</param>
        /// <param name="pageSize">Number of games per page</param>
        /// <returns>Returns all <see cref="GameWithoutRatingsDto"/>, containing search query and paginated.</returns>
        /// <response code="200">Returns <see cref="GameWithoutRatingsDto"/> for all games matching parameters and paginated.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameWithoutRatingsDto>>> GetAllGames(string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxGamesPageSize)
            {
                pageSize = maxGamesPageSize;
            }
            
            var (gameEntities, paginationMetadata) = await _playNoteRpository.GetAllGamesAsync(searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<GameWithoutRatingsDto>>(gameEntities));
        }
    }
}
