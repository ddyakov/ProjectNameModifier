var projectInfo = UserInput.AskForProjectInfo();

try
{
    WriteLine($"Renaming project in directory: {projectInfo.SolutionDirectory}...");
    new Renamer(projectInfo).RenameProject();
    WriteLine($"Successfully renamed project.");
}
catch (Exception e)
{
    WriteLine($"Something went wrong. Reason: {e.Message}");
}

ReadLine();
