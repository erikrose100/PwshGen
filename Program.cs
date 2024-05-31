using System.CommandLine;
using System.Text;

namespace PwshGen;
class Program
{
    static async Task<int> Main(string[] args)
    {
        var fileName = new Option<string?>(
            name: "--file",
            description: "File name to use for output -- defaults to cwd name and appends a random int if it already exists.",
            getDefaultValue: () =>  Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar).Last());

        var paramName = new Option<string?>(
            name: "--param",
            description: "Name to use for main function parameter",
            getDefaultValue: () => Environment.GetEnvironmentVariable("PWSHGEN_MAIN_PARAM") ?? "param");

        var creatorName = new Option<string?>(
            name: "--creator-name",
            description: "Name of creator to put in the file.",
            getDefaultValue: () => Environment.GetEnvironmentVariable("PWSHGEN_NAME") ?? "<Name>");

        var template = new Option<string?>(
            name: "--template",
            description: ".psmd template file to read from",
            getDefaultValue: () => Environment.GetEnvironmentVariable("PWSHGEN_TEMPLATE_FILE") ?? null);

        var rootCommand = new RootCommand("Sample app for System.CommandLine")
        {
            fileName,
            paramName,
            creatorName,
            template
        };

        rootCommand.SetHandler((fileName, paramName, creatorName, template) =>
            {
                if (template is null)
                {
                    var outputTemplate = new Template(paramName!, "\n\n\tbegin {\n\t}\n\n\tprocess {\n\t}\n\n\tend {\n\t}", creatorName!, DateTime.Now);
                    StringBuilder output = new();
                    output
                        .Append(outputTemplate.requires)
                        .Append(outputTemplate.help)
                        .Append(outputTemplate.cmdletBinding)
                        .Append(outputTemplate.scriptParams)
                        .Append(outputTemplate.subFunction)
                        .Append(outputTemplate.mainFunction)
                        .Append(outputTemplate.run);
                    OutputFile(fileName!, output);
                }
                else
                {
                    var markdownText = File.ReadAllText(template);
                    var options = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
                    var sections = markdownText.Split(":::", options);
                    var outputTemplate = new Dictionary<string, string>();
                    foreach (var section in sections)
                    {
                        int charLocation = section.IndexOf('\n');
                        string header = section[..charLocation];
                        string sectionValue = section.Remove(0, charLocation);
                        outputTemplate.Add(header, sectionValue);
                    }
                    StringBuilder output = new();
                    output
                        .Append(outputTemplate["requires"])
                        .Append(outputTemplate["help"])
                        .Append(outputTemplate["cmdletBinding"])
                        .Append(outputTemplate["scriptParams"])
                        .Append(outputTemplate["subFunction"])
                        .Append(outputTemplate["mainFunction"])
                        .Append(outputTemplate["run"]);
                    OutputFile(fileName!, output);
                }
            },
            fileName, paramName, creatorName, template);

        return await rootCommand.InvokeAsync(args);
    }
    static void OutputFile(string fileName, StringBuilder output)
    {
        if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $"{fileName!}.ps1")))
        {
            Random rand = new();
            byte[] bytes = new byte[1];
            rand.NextBytes(bytes);
            var byteString = string.Join("", bytes);
            fileName += byteString;
        }
        using (FileStream outputFile = new(Path.Combine(Directory.GetCurrentDirectory(), $"{fileName!}.ps1"), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
        {
            using StreamWriter outputStream = new(outputFile);
            outputStream.WriteLine(output);
        };
    }
}
