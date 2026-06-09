using FluentValidation;
using pollbackend.Application.DTOs;
using System.Linq;

namespace pollbackend.Application.Validators
{
    public class CreatePollDtoValidator : AbstractValidator<CreatePollDto>
    {
        public CreatePollDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Poll title is required.")
                .MaximumLength(255).WithMessage("Poll title cannot exceed 255 characters.");

            RuleFor(x => x.Options)
                .NotNull().WithMessage("Options cannot be null.")
                .Must(options => options != null && options.Count >= 2)
                .WithMessage("A poll must have at least 2 options.")
                .Must(options => options != null && options.All(opt => !string.IsNullOrWhiteSpace(opt)))
                .WithMessage("All option texts must be non-empty.")
                .Must(options => options != null && options.All(opt => opt.Length <= 255))
                .WithMessage("Each option text cannot exceed 255 characters.");
        }
    }
}
