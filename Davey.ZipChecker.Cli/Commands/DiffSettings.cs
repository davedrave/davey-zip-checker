using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace DaveyZipChecker.Cli.Commands;

public sealed class DiffSettings : CommandSettings
{
    [CommandOption("--zip <ZIP>")]
    [Description("Path to a ZIP file (can be specified multiple times)")]
    public string[] ZipPaths { get; set; } = Array.Empty<string>();

    [CommandOption("--folder <FOLDER>")]
    [Description("Path to the folder")]
    public string FolderPath { get; init; } = string.Empty;


    [CommandOption("--strip-zip-root <PATH>")]
    [Description("Strip this leading path from ZIP entries")]
    public string? StripZipRoot { get; init; }


    public override ValidationResult Validate()
    {
        if (ZipPaths.Length == 0)
            return ValidationResult.Error("At least one --zip must be specified.");

        foreach (string zip in ZipPaths)
        {
            if (!File.Exists(zip))
                return ValidationResult.Error($"ZIP file not found: {zip}");
        }

        if (!Directory.Exists(FolderPath))
            return ValidationResult.Error($"Folder not found: {FolderPath}");

        return ValidationResult.Success();
    }
}
