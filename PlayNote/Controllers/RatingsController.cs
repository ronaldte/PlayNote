using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PlayNote.Entities;
using PlayNote.Models;
using PlayNote.Services;
using System.Text.Json;

namespace PlayNote.Controllers
{
    /// <summary>
    /// Routes for controlling rating.
    /// </summary>
    [Route("api/Games/{gameId:int}/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private const int maxRatingsPageSize = 20;

        private readonly IPlayNoteRepository _playNoteRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RatingsController> _logger;
        private readonly IValidator<Rating> _validator;

        /// <summary>
        /// Creates new instance of ratings controller.
        /// </summary>
        /// <param name="playNoteRepository">Repository for accessing ratings.</param>
        /// <param name="mapper">Mapper for transformig entities to dtos.</param>
        /// <param name="logger">Logger for logging events.</param>
        /// <param name="validator">Validator for <see cref="Rating"/> entity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RatingsController(IPlayNoteRepository playNoteRepository, IMapper mapper, ILogger<RatingsController> logger, IValidator<Rating> validator)
        {
            _playNoteRepository = playNoteRepository ?? throw new ArgumentNullException(nameof(playNoteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Returns single entity of rating for given game.
        /// </summary>
        /// <param name="gameId">Id of the game for which the rating is.</param>
        /// <param name="ratingId">Id of the rating to be returned.</param>
        /// <returns><see cref="RatingDto"/> for rating.</returns>
        /// <response code="200">Returns the requested rating.</response>
        /// <response code="404">Either game or rating was not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{ratingId:int}", Name = "GetRating")]
        public async Task<ActionResult<RatingDto>> GetRating(int gameId, int ratingId)
        {
            if (!await _playNoteRepository.GameExistsAsync(gameId))
            {
                return NotFound($"Game with Id {gameId} does not exist.");
            }

            var ratingEntity = await _playNoteRepository.GetRatingForGameAsync(gameId, ratingId);
            if (ratingEntity == null)
            {
                return NotFound($"Rating with Id {ratingId} does not exist.");
            }

            return Ok(_mapper.Map<RatingDto>(ratingEntity));
        }

        /// <summary>
        /// Returns a list of all ratings for given game.
        /// </summary>
        /// <param name="gameId">Id of the game for which the rating is.</param>
        /// <param name="searchQuery">String used to search in ratings.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Size of the page for pagination.</param>
        /// <returns>List of all ratings for given game.</returns>
        /// <response code="200">Returns the requested rating, with rewviews containing search query and result paginated.</response>
        /// <response code="404">Game was not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatings(int gameId, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (!await _playNoteRepository.GameExistsAsync(gameId))
            {
                return NotFound($"Game with Id {gameId} does not exist.");
            }

            if (pageSize > maxRatingsPageSize)
            {
                pageSize = maxRatingsPageSize;
            }

            var (ratingEntities, paginationMetadata) = await _playNoteRepository.GetAllRatingsForGameAsync(gameId, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<RatingDto>>(ratingEntities));
        }

        /// <summary>
        /// Creates new rating entity.
        /// </summary>
        /// <param name="gameId">Id of the game for which the rating is.</param>
        /// <param name="rating"><see cref="RatingForCreationDto"/> model representing new rating.</param>
        /// <returns>Created <see cref="RatingDto"/> object for created rating.</returns>
        /// <response code="201">Created at route.</response>
        /// <response code="400">Rating is not valid.</response>
        /// <response code="404">Game with given id was not found.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RatingDto>> CreateRating(int gameId, RatingForCreationDto rating)
        {
            if (!await _playNoteRepository.GameExistsAsync(gameId))
            {
                return NotFound($"Game with Id {gameId} does not exist.");
            }

            var ratingEntity = _mapper.Map<Rating>(rating);
            var validationResult = await _validator.ValidateAsync(ratingEntity);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            await _playNoteRepository.AddRatingAsync(gameId, ratingEntity);
            await _playNoteRepository.SaveChangesAsync();

            _logger.LogInformation("Created new rating entry {@ratingEntity} for game id {gameId}", ratingEntity, gameId);

            var addedRating = _mapper.Map<RatingDto>(ratingEntity);

            return CreatedAtRoute("GetRating", new { gameId = gameId, ratingId = addedRating.Id }, addedRating);
        }

        /// <summary>
        /// Fully updates specified rating with new properties.
        /// </summary>
        /// <param name="gameId">Id of the game for which the rating is.</param>
        /// <param name="ratingId">Id of the rating to be updated.</param>
        /// <param name="rating"><see cref="RatingForUpdateDto"/> model with updated properties.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Rating entity successfully updated.</response>
        /// <response code="400">Rating is not valid.</response>
        /// <response code="404">Either game or rating was not found.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{ratingId:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateRating(int gameId, int ratingId, RatingForUpdateDto rating)
        {
            if (!await _playNoteRepository.GameExistsAsync(gameId))
            {
                return NotFound($"Game with Id {gameId} does not exist.");
            }

            var ratingEntity = await _playNoteRepository.GetRatingForGameAsync(gameId, ratingId);
            if (ratingEntity == null)
            {
                return NotFound($"Rating with Id {ratingId} does not exist.");
            }

            _mapper.Map(rating, ratingEntity);
            var validationResult = await _validator.ValidateAsync(ratingEntity);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            await _playNoteRepository.SaveChangesAsync();

            _logger.LogInformation("Updated rating id {ratingId} for game id {gameId} to {@ratingEntity}", ratingId, gameId, ratingEntity);

            return NoContent();
        }

        /// <summary>
        /// Partially updates given rating.
        /// </summary>
        /// <param name="gameId">Id of the game for which the rating is.</param>
        /// <param name="ratingId">Id of the rating to be updated.</param>
        /// <param name="patchDocument">JSONPatch document with operations on <see cref="RatingForUpdateDto"/> model.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Rating entity successfully updated.</response>
        /// <response code="400">Either JSONPatch document or rating entity itself is not valid.</response>
        /// <response code="404">Either game or rating was not found.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{ratingId:int}")]
        [Authorize]
        public async Task<ActionResult> PartiallyUpdateRating(int gameId, int ratingId, JsonPatchDocument<RatingForUpdateDto> patchDocument)
        {
            if (!await _playNoteRepository.GameExistsAsync(gameId))
            {
                return NotFound($"Game with Id {gameId} does not exist.");
            }

            var ratingEntity = await _playNoteRepository.GetRatingForGameAsync(gameId, ratingId);
            if (ratingEntity == null)
            {
                return NotFound($"Rating with Id {ratingId} does not exist.");
            }

            var ratingToPatch = _mapper.Map<RatingForUpdateDto>(ratingEntity);
            patchDocument.ApplyTo(ratingToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(ratingToPatch, ratingEntity);

            var validationResult = await _validator.ValidateAsync(ratingEntity);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            await _playNoteRepository.SaveChangesAsync();

            _logger.LogInformation("Updated rating id {ratingId} for game id {gameId} to {@ratingEntity}", ratingId, gameId, ratingEntity);

            return NoContent();
        }

        /// <summary>
        /// Deletes existing rating entity.
        /// </summary>
        /// <param name="gameId">Id of the game for which the rating is.</param>
        /// <param name="ratingId">Id of the rating to be deleted.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Rating successfully deleted.</response>
        /// <response code="404">Either game or rating was not found.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{ratingId:int}")]
        [Authorize("Admin")]
        public async Task<ActionResult> DeleteRating(int gameId, int ratingId)
        {
            if (!await _playNoteRepository.GameExistsAsync(gameId))
            {
                return NotFound($"Game with Id {gameId} does not exist.");
            }

            var ratingEntity = await _playNoteRepository.GetRatingForGameAsync(gameId, ratingId);
            if (ratingEntity == null)
            {
                return NotFound($"Rating with Id {ratingId} does not exist.");
            }

            _playNoteRepository.DeleteRating(ratingEntity);
            await _playNoteRepository.SaveChangesAsync();

            _logger.LogInformation("Deleted rating id {ratingId} for game id {gameId}", ratingId, gameId);

            return NoContent();
        }
    }
}
