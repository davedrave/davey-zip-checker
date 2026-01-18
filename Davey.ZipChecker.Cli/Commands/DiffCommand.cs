using Spectre.Console.Cli;
using System;

namespace DaveyZipChecker.Cli.Commands;

public class DiffCommand : Command<DiffSettings>
{
    public override int Execute(CommandContext context, DiffSettings settings, CancellationToken cancellationToken)
    {
        // TEMP: just to prove it wires up
        Console.WriteLine("Diff command executed");
        Console.WriteLine($"ZIP: {settings.ZipPath}");
        Console.WriteLine($"Folder: {settings.FolderPath}");

        return 0;
    }
}
