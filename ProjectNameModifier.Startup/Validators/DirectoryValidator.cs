namespace ProjectNameModifier.Startup.Validators;

using FluentValidation;

internal class DirectoryValidator : AbstractValidator<string>
{
    public DirectoryValidator()
    {
        Include(new RequiredStringValidator());

        RuleFor(it => it)
            .Must(it => Directory.Exists(it))
            .WithMessage("Directory does not exist.");
    }
}
