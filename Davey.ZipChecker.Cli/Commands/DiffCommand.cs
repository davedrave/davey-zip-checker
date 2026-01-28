using Davey.ZipChecker;
using Davey.ZipChecker.Cli;
using Davey.ZipChecker.Core;
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
        // Determine zip path: prefer first argument, otherwise prompt interactively.
        IReadOnlyList<string> zipPaths = settings.ZipPaths;
        string folderPath = settings.FolderPath;

        Console.WriteLine($"Comparing {zipPaths.Count} ZIP(s) to folder:");
        Console.WriteLine($"  {folderPath}");
        Console.WriteLine();

        List<IReadOnlyList<ZipEntryInfo>> zipEntries = new();

        for (int i = 0; i < zipPaths.Count; i++)
        {
            Console.WriteLine($"[{i + 1}/{zipPaths.Count}] Scanning ZIP: {Path.GetFileName(zipPaths[i])}");

            ListOptions zipOptions = new ListOptions
            {
                StripRoot = settings.StripZipRoot
            };


            IReadOnlyList<ZipEntryInfo> entries = new ZipContentLister().ListContents(zipPaths[i], zipOptions);
            zipEntries.Add(entries);

            Console.WriteLine($"    Found {entries.Count:N0} files");
        }

        Console.WriteLine();

        Console.WriteLine("Scanning folder...");
        ConsoleScanProgress consoleScanProgress = new();
        var folderEntries = new FolderContentLister().ListContents(folderPath, null, consoleScanProgress);

        Console.WriteLine($"Folder scan complete ({folderEntries.Count:N0} files).");
        Console.WriteLine();

        // Compute and display diff
        for (int i = 0; i < zipEntries.Count; i++)
        {
            Console.WriteLine($"Diffing ZIP [{i + 1}/{zipEntries.Count}]");
            var diff = DiffChecker.ComputeDiff
            (
                zipEntries[i],
                folderEntries,
                e => ComparisonPathNormaliser.Normalise(e.Path),
                StringComparer.OrdinalIgnoreCase
            );

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
        #if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        #endif
        return 0;

    }
}
