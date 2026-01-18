using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Davey.ZipChecker;

internal static class Program1
{
    private static int Main(string[] args)
    {
        // Determine zip path: prefer first argument, otherwise prompt interactively.
        string zipPath = "C:\\Temp\\Temp.zip";
        string folderPath = "C:\\Temp\\Temp\\";

        var zipEntries = ContentListerFactory.Create(zipPath).ListContents(zipPath);
        var folderEntries = ContentListerFactory.Create(folderPath).ListContents(folderPath);

        Console.WriteLine($"Contents of {Path.GetFullPath(zipPath)} ({zipEntries.Count} entries):");
        foreach (var e in zipEntries)
        {
            var flag = e.IsDirectory ? "[D]" : "[F]";
            Console.WriteLine($"{flag} {e.Path}");
        }

        Console.WriteLine();
        Console.WriteLine($"Contents of {Path.GetFullPath(folderPath)} ({folderEntries.Count} entries):");
        foreach (var e in folderEntries)
        {
            var flag = e.IsDirectory ? "[D]" : "[F]";
            Console.WriteLine($"{flag} {e.Path}");
        }

        // Compute and display diff
        var diff = DiffChecker.ComputeDiff(zipEntries, folderEntries);

        Console.WriteLine();
        Console.WriteLine($"Only in ZIP ({diff.OnlyInA.Count}):");
        if (diff.OnlyInA.Count == 0)
            Console.WriteLine("  (none)");
        else
        {
            foreach (var e in diff.OnlyInA)
            {
                var flag = e.IsDirectory ? "[D]" : "[F]";
                Console.WriteLine($"  {flag} {e.Path}");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Only in Folder ({diff.OnlyInB.Count}):");
        if (diff.OnlyInB.Count == 0)
            Console.WriteLine("  (none)");
        else
        {
            foreach (var e in diff.OnlyInB)
            {
                var flag = e.IsDirectory ? "[D]" : "[F]";
                Console.WriteLine($"  {flag} {e.Path}");
            }
        }

        Console.WriteLine();

        // Wait for the user to press a key before exiting.
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(true);

        return 0;
    }
}
