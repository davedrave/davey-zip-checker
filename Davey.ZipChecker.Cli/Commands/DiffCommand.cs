using Davey.ZipChecker;
using Spectre.Console.Cli;
using System;
using System.Threading;

namespace DaveyZipChecker.Cli.Commands;

public sealed class DiffCommand : Command<DiffSettings>
{
    public override int Execute(
        CommandContext context,
        DiffSettings settings,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Diff command executed");
        foreach (string zipPath in settings.ZipPaths)
        {
            Console.WriteLine($"Processing ZIP: {zipPath}");
            // call core diff logic here
        }
        Console.WriteLine($"Folder: {settings.FolderPath}");

        // Determine zip path: prefer first argument, otherwise prompt interactively.
        IReadOnlyList<string> zipPaths = settings.ZipPaths;
        string folderPath = settings.FolderPath;

        List<IReadOnlyList<ZipEntryInfo>> zipEntries = new();

        foreach (string zipPath in zipPaths)
        {
            var entries = ContentListerFactory.Create(zipPath).ListContents(zipPath);
            zipEntries.Add(entries);

            Console.WriteLine($"{zipEntries.Count} entries found in {Path.GetFullPath(zipPath)} ():");
        }
        var folderEntries = ContentListerFactory.Create(folderPath).ListContents(folderPath);

        // Compute and display diff
        foreach (var e in zipEntries)
        {
            var diff = DiffChecker.ComputeDiff(e, folderEntries);

            Console.WriteLine();
            Console.WriteLine($"Only in ZIP ({diff.OnlyInA.Count}):");
            if (diff.OnlyInA.Count == 0)
                Console.WriteLine("  (none)");
            else
            {
                foreach (var f in diff.OnlyInA)
                {
                    var flag = f.IsDirectory ? "[D]" : "[F]";
                    Console.WriteLine($"  {flag} {f.Path}");
                }
            }

        }

        Console.WriteLine();

        // Wait for the user to press a key before exiting.
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(true);

        return 0;

    }
}
