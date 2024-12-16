using System.CommandLine;
using System.Text;

var rootCommand = new RootCommand("bundle files");
var bundleCommand = new Command("bundle", "bundle files into one file");
var createRspCommand = new Command("create-rsp", "create response file");

var supportedLanguages = new Dictionary<string, string> {
    {"python",".py" },
    {"csharp",".cs" },
    {"java",".java" },
    {"sql",".sql" },
    {"js",".js" },
    {"html",".html" },
    {"c++",".cpp" },
    {"css",".css" },
    { "text", "txt" }
};

var outputFileOption = new Option<FileInfo>("--output", "file path and name") { IsRequired = true };
var removeEmptyLinesOption = new Option<bool>("--remove-empty-lines", "remove empty lines befor bundling");
var authorOption = new Option<string>("--author", "Name of the author of the code file");
var noteOption = new Option<bool>("--note", "to put the source code in the output file");
var languageFileOption = new Option<string[]>("--language", "choose languages or all") { AllowMultipleArgumentsPerToken = true }
.FromAmong(supportedLanguages.Keys.Concat(new[] { "all" }).ToArray());
var sortOption = new Option<string>("--sort", "Sort files by 'name' or 'type' (default is 'name')")
{
    Arity = ArgumentArity.ZeroOrOne
}.FromAmong("name", "type");


bundleCommand.AddOption(languageFileOption);
bundleCommand.AddOption(removeEmptyLinesOption);
bundleCommand.AddOption(outputFileOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(authorOption);


bundleCommand.SetHandler((output, languages, includeNote, sortBy, isAuthor, removeLines) =>
{
    try
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var allfile = Directory.GetFiles(currentDirectory);

        var selectedExtensions = languages.Contains("all") ? supportedLanguages.Values.ToList() : languages
        .Where(lang => supportedLanguages.ContainsKey(lang))
        .Select(lang => supportedLanguages[lang])
        .ToList();

        var filteredFiles = allfile
           .Where(file => selectedExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
           .ToList();

        filteredFiles = sortBy switch
        {
            "type" => filteredFiles.OrderBy(file => Path.GetExtension(file)).ToList(),
            _ => filteredFiles.OrderBy(file => Path.GetFileName(file)).ToList(),
        };
        var allContent = new StringBuilder();
        if (isAuthor != null)
        {
            allContent.Append($"/*author: {isAuthor}*/\n\n");
        }

        foreach (var file in filteredFiles)
        {
            string[] contentFile = File.ReadAllLines(file);
            if (includeNote)
            {
                allContent.Append($"/* file name: {Path.GetFileName(file)} \n source: {Path.GetRelativePath(currentDirectory, file)} */\n\n");
            }
            if (removeLines)
                contentFile = contentFile.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
            allContent.Append(string.Join('\n', contentFile) + "\n \n");
            allContent.Append(contentFile + "\n \n");
        }
        File.WriteAllText(output.FullName, allContent.ToString());
        Console.WriteLine("file bundled successfully!");

    }
    catch (DirectoryNotFoundException ex)
    {
        Console.WriteLine("Directory not found");
    }

}, outputFileOption, languageFileOption, noteOption, sortOption, authorOption, removeEmptyLinesOption);


createRspCommand.SetHandler(() =>
{
    Console.WriteLine("What's the File name for output?");
    string fileName = Console.ReadLine();

    Console.WriteLine("Would you like to include all files? Insert Y or N");
    bool allFiles = Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase);

    Console.WriteLine("If you don't want to include all files, which languages to include? (separate by space)");
    string languages = Console.ReadLine();

    Console.WriteLine("Would you like to put a note? Insert Y or N");
    bool note = Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase);

    Console.WriteLine("Insert the sort: type/name");
    string sort = Console.ReadLine();

    Console.WriteLine("Would you like to remove empty lines? Insert Y or N");
    bool removeLines = Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase);

    Console.WriteLine("Insert the aouthor name");
    string aouthor = Console.ReadLine();

    var sb = new StringBuilder();
    sb.Append($"bundle --output \"{fileName}\" ");

    if (allFiles)
        sb.Append("--language all ");
    else
        sb.Append($"--language {languages} ");

    if (removeLines)
        sb.Append("--remove-empty-lines ");

    if (note)
        sb.Append("--note ");

    if (aouthor != null)
        sb.Append($"--author {aouthor}");

    string fullCommand = sb.ToString();
    Console.WriteLine("What's the File name for the rsponse file?");
    string rspName = Console.ReadLine();
    string responseFilePath = $"{rspName}.rsp";
    File.WriteAllText(responseFilePath, fullCommand);
    Console.WriteLine($"Ok! All is done. Your job is to type the command: fim @{responseFilePath}");
});

outputFileOption.AddAlias("--o");
languageFileOption.AddAlias("--l");
noteOption.AddAlias("--n");
sortOption.AddAlias("--s");
removeEmptyLinesOption.AddAlias("--rm");
authorOption.AddAlias("--a");

rootCommand.Add(bundleCommand);
rootCommand.Add(createRspCommand);
rootCommand.Invoke(args);
