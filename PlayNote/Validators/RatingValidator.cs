using FluentValidation;
using PlayNote.Entities;

namespace PlayNote.Validators
{
    /// <summary>
    /// Validation rules for <see cref="Rating"/> entity.
    /// </summary>
    public class RatingValidator : AbstractValidator<Rating>
    {
        /// <summary>
        /// Creates rules for validating <see cref="Rating"/> entity.
        /// </summary>
        public RatingValidator()
        {
            RuleFor(r => r.Points)
                .NotNull()
                .NotEmpty().WithMessage("Please provide a value for review {PropertyName}.")
                .InclusiveBetween(1, 100).WithMessage("Please ensure review {PropertyName} be between 1 and 100.");

            RuleFor(r => r.Review)
                .MaximumLength(1024).WithMessage("Please ensure maximum length for {PropertyName} is {MaxLength}.");
        }
    }
}
