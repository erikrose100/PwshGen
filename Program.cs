using System.CommandLine;
using System.Text;
using System.ComponentModel;
using System.Linq;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Markdig.Extensions.CustomContainers;

namespace PwshGen;
class Program
{
    static async Task<int> Main(string[] args)
    {
        var fileName = new Option<string?>(
            name: "--file",
            description: "The file to read and display on the console.",
            getDefaultValue: () => "pwsh");

        var paramName = new Option<string?>(
            name: "--param",
            description: "The file to read and display on the console.",
            getDefaultValue: () => "param");

        var template = new Option<string?>(
            name: "--template",
            description: "Specify template to use instead of default.");

        var rootCommand = new RootCommand("Sample app for System.CommandLine")
        {
            fileName,
            paramName,
            template
        };

        rootCommand.SetHandler((fileName, paramName, template) =>
            {
                if (template is null && Environment.GetEnvironmentVariable("PWSHGEN_TEMPLATE_FILE") is not null)
                {
                    template = Environment.GetEnvironmentVariable("PWSHGEN_TEMPLATE_FILE");
                }
                if (template is null)
                {
                    var outputTemplate = new Template(paramName!, "\n\n\tbegin {\n\t}\n\n\tprocess {\n\t}\n\n\tend {\n\t}");
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
            fileName, paramName, template);

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
