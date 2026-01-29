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


    [CommandOption("--username <USERNAME>")]
    [Description("Username for network share authentication (optional)")]
    public string? Username { get; set; }

    [CommandOption("--password <PASSWORD>")]
    [Description("Password for network share authentication (optional)")]
    public string? Password { get; set; }

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

        // Validate that if username is provided, password must also be provided
        if (!string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password))
            return ValidationResult.Error("Password must be provided when username is specified.");

        if (!string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(Username))
            return ValidationResult.Error("Username must be provided when password is specified.");

        return ValidationResult.Success();
    }
}