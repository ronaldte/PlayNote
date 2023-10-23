using FluentValidation;
using PlayNote.Entities;

namespace PlayNote.Validators
{
    /// <summary>
    /// Validation rules for <see cref="Game"/> entity.
    /// </summary>
    public class GameValidator : AbstractValidator<Game>
    {
        /// <summary>
        /// Creates rules for validating <see cref="Game"/> entity.
        /// </summary>
        public GameValidator()
        {
            RuleFor(g => g.Name)
                .NotNull()
                .NotEmpty().WithMessage("Please include {PropertyName} for the game.")
                .MinimumLength(3).WithMessage("Please ensure {PropertyName} is at least {MinLength}.")
                .MaximumLength(64).WithMessage("Please ensure {PropertyName} is at max {MaxLength}.");

            RuleFor(g => g.Description)
                .MaximumLength(1024).WithMessage("Please ensure {PropertyName} is at max {MaxLength}.");
        }
    }
}
