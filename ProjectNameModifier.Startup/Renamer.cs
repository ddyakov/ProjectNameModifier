namespace ProjectNameModifier.Startup;

using System.Text;

internal class Renamer
{
    private readonly List<string> _skipFolders;

    private readonly ProjectInfo _projectInfo;
    private readonly string? _currentDirectory;

    private string? _currentProjectName;

    public Renamer(ProjectInfo projectInfo)
    {
        _projectInfo = projectInfo;
        _currentDirectory = _projectInfo.SolutionDirectory;

        _skipFolders = new List<string>
        {
            $"{_currentDirectory}\\.git",
            $"{_currentDirectory}\\.vs",
            $"{_currentDirectory}\\bin",
            $"{_currentDirectory}\\obj"
        };

        SetCurrentProjectName();
    }

    public void RenameProject()
    {
        RenameDirectories(_currentDirectory!);
        RenameFiles(_currentDirectory!);
        RenameFilesContent(_currentDirectory!);

        var newRootDirectory = _currentDirectory!.Replace(_currentProjectName!, _projectInfo.NewProjectName);

        Directory.Move(_currentDirectory!, newRootDirectory);
    }

    private void RenameDirectories(string currentDirectory)
    {
        VerifyAndRenameValues(GetFilteredDirectories(currentDirectory));

        LoopDirectories(currentDirectory, RenameDirectories);
    }

    private void RenameFiles(string currentDirectory)
    {
        VerifyAndRenameValues(Directory.GetFiles(currentDirectory));

        LoopDirectories(currentDirectory, RenameFiles);
    }

    private void RenameFilesContent(string currentDirectory)
    {
        var files = Directory.GetFiles(currentDirectory);

        foreach (var file in files)
        {
            if (file.EndsWith(".exe") &&
                file.EndsWith(".dll") &&
                file.EndsWith(".runtimeconfig.json"))
                continue;

            var content = File.ReadAllText(file);

            File.WriteAllText(
                file,
                content.Replace(
                    _currentProjectName!,
                    _projectInfo.NewProjectName),
                Encoding.UTF8);
        }

        LoopDirectories(currentDirectory, RenameFilesContent);
    }

    private void LoopDirectories(string currentDirectory, Action<string> action)
    {
        var directories = GetFilteredDirectories(currentDirectory);

        foreach (var directory in directories)
            action(directory);
    }

    private void SetCurrentProjectName()
    {
        var rootFiles = Directory.GetFiles(_currentDirectory!);

        _currentProjectName = Path.GetFileName(rootFiles
            .FirstOrDefault(file => file.EndsWith(".sln")))
            ?.Replace(".sln", string.Empty);

        if (string.IsNullOrEmpty(_currentProjectName))
            throw new NullReferenceException("The current project name could not be detected.");

        if (_projectInfo.NewProjectName == _currentProjectName)
            throw new Exception("The new project name is the same, nothing was changed.");
    }

    private void VerifyAndRenameValues(string[] values)
    {
        foreach (var value in values)
        {
            var valueSplit = value.Split("\\");
            var lastValue = valueSplit.LastOrDefault();

            if (string.IsNullOrEmpty(lastValue) ||
                !lastValue.Contains(_currentProjectName!))
                continue;

            valueSplit[^1] = lastValue.Replace(
                _currentProjectName!, 
                _projectInfo.NewProjectName);

            Directory.Move(
                value,
                Path.Combine(valueSplit));
        }
    }

    private string[] GetFilteredDirectories(string currentDirectory)
        => Directory
            .GetDirectories(currentDirectory)
            .Except(_skipFolders)
            .ToArray();
}
