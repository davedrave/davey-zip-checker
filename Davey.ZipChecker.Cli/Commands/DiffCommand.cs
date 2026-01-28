using Davey.ZipChecker;
using Davey.ZipChecker.Cli;
using Davey.ZipChecker.Core;
using Davey.ZipChecker.Core.Networking;
using Spectre.Console.Cli;
using System;
using System.Diagnostics;
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

        // Disconnect existing connections if we're providing credentials
        if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
        {
            DisconnectExistingConnections(folderPath);
        }

        // Create authentication provider only if credentials are provided
        IPathAuthenticationProvider? authProvider = CreateAuthProvider(settings);

        List<IReadOnlyList<ZipEntryInfo>> zipEntries = new();
        for (int i = 0; i < zipPaths.Count; i++)
        {
            Console.WriteLine($"[{i + 1}/{zipPaths.Count}] Scanning ZIP: {Path.GetFileName(zipPaths[i])}");

            ZipContentLister zipContentLister = new ZipContentLister();
            IReadOnlyList<ZipEntryInfo> entries = zipContentLister.ListContents(zipPaths[i], cancellationToken: cancellationToken);
            zipEntries.Add(entries);
            Console.WriteLine($"    Found {entries.Count:N0} files");
        }

        Console.WriteLine();
        Console.WriteLine("Scanning folder...");
        ConsoleScanProgress consoleScanProgress = new();

        FolderContentLister folderContentLister = new FolderContentLister(authProvider);
        var folderEntries = folderContentLister.ListContents(folderPath, consoleScanProgress, cancellationToken);

        Console.WriteLine($"Folder scan complete ({folderEntries.Count:N0} files).");
        Console.WriteLine();

        // Compute and display diff
        for (int i = 0; i < zipEntries.Count; i++)
        {
            Console.WriteLine($"Diffing ZIP [{i + 1}/{zipEntries.Count}]");
            var diff = DiffChecker.ComputeDiff(zipEntries[i], folderEntries);
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

    private static void DisconnectExistingConnections(string folderPath)
    {
        if (!folderPath.StartsWith(@"\\"))
            return;

        var parts = folderPath.TrimStart('\\').Split('\\');
        if (parts.Length < 1)
            return;

        string server = parts[0];

        Console.WriteLine($"[Auth] Disconnecting ALL connections to \\\\{server}...");

        // Method 1: Disconnect using wildcard (all connections)
        ExecuteNetCommand($"use * /delete /yes");
        Thread.Sleep(300);

        // Method 2: Disconnect specific server
        ExecuteNetCommand($"use \\\\{server} /delete /yes");
        Thread.Sleep(300);

        // Method 3: Disconnect IPC$ (administrative share)
        ExecuteNetCommand($"use \\\\{server}\\IPC$ /delete /yes");
        Thread.Sleep(500);

        Console.WriteLine($"[Auth] Disconnection complete. Waiting for Windows to clean up...");
        Thread.Sleep(1000); // Give Windows even more time
    }

    private static void ExecuteNetCommand(string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "net",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(psi);
            if (process != null)
            {
                process.WaitForExit();
                Console.WriteLine($"[Auth]   net {arguments} -> Exit code: {process.ExitCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Auth]   Warning: {ex.Message}");
        }
    }

    private static IPathAuthenticationProvider? CreateAuthProvider(DiffSettings settings)
    {
        // Priority 1: Use explicit credentials from command line
        if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
        {
            Console.WriteLine("[Auth] Using provided credentials for network authentication");
            return new NetworkShareAuthenticationProvider(
                settings.Username,
                settings.Password);
        }

        // Priority 2: Check environment variables
        string? envUsername = Environment.GetEnvironmentVariable("NETWORK_USERNAME");
        string? envPassword = Environment.GetEnvironmentVariable("NETWORK_PASSWORD");
        string? envDomain = Environment.GetEnvironmentVariable("NETWORK_DOMAIN");

        if (!string.IsNullOrEmpty(envUsername) && !string.IsNullOrEmpty(envPassword))
        {
            Console.WriteLine("[Auth] Using credentials from environment variables");
            return new NetworkShareAuthenticationProvider(envUsername, envPassword, envDomain);
        }

        // Priority 3: Return null - will use default behavior (current Windows credentials)
        return null;
    }
}