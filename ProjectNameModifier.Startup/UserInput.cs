namespace ProjectNameModifier.Startup;

using FluentValidation;
using FluentValidation.Results;

using Validators;

internal static class UserInput
{
    public static ProjectInfo AskForProjectInfo()
    {
        var newProjectName = AskForInput("New project name: ", new RequiredStringValidator());
        var projectDirectory = AskForInput("Solution path: ", new DirectoryValidator());

        return new ProjectInfo(newProjectName, projectDirectory);
    }

    private static string AskForInput(string headline, IValidator<string> validator)
    {
        ValidationResult result;
        string? input;

        do
        {
            if (!string.IsNullOrEmpty(headline))
                Write(headline);

            input = ReadLine();
            result = validator.Validate(input!);

            if (result.Errors.Any())
                WriteLine(string.Join("\n", result.Errors));
        }
        while (!result.IsValid);

        return input!;
    }
}
