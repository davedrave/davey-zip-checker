using Spectre.Console.Cli;
using System.ComponentModel;

namespace DaveyZipChecker.Cli.Commands;

public sealed class DiffSettings : CommandSettings
{
    [CommandOption("--zip <ZIP>")]
    [Description("Path to the ZIP file")]
    public string ZipPath { get; init; } = string.Empty;

    [CommandOption("--folder <FOLDER>")]
    [Description("Path to the folder")]
    public string FolderPath { get; init; } = string.Empty;
}
