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

            Console.WriteLine($"Contents of {Path.GetFullPath(zipPath)} ({zipEntries.Count} entries):");
            foreach (var e in zipEntries)
            {
                foreach (var f in e)
                {
                    var flag = f.IsDirectory ? "[D]" : "[F]";
                    Console.WriteLine($"{flag} {f.Path}");
                }
            }
        }
        var folderEntries = ContentListerFactory.Create(folderPath).ListContents(folderPath);



        Console.WriteLine();
        Console.WriteLine($"Contents of {Path.GetFullPath(folderPath)} ({folderEntries.Count} entries):");
        foreach (var e in folderEntries)
        {
            var flag = e.IsDirectory ? "[D]" : "[F]";
            Console.WriteLine($"{flag} {e.Path}");
        }

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

            Console.WriteLine();
            Console.WriteLine($"Only in Folder ({diff.OnlyInB.Count}):");
            if (diff.OnlyInB.Count == 0)
                Console.WriteLine("  (none)");
            else
            {
                foreach (var f in diff.OnlyInB)
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
