namespace ProjectNameModifier.Startup.Validators;

using FluentValidation;

internal class RequiredStringValidator : AbstractValidator<string>
{
    public RequiredStringValidator()
    {
        RuleFor(it => it)
            .NotEmpty()
            .WithMessage("Value must not be empty.")
            .NotNull()
            .WithMessage("Value must not be null.");
    }
}
