using Davey.ZipChecker;
using Davey.ZipChecker.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

public class ZipContentLister : IContentLister
{
    /// <summary>
    /// Lists the entries in a zip file.
    /// Normalizes path separators to '/' and marks directory entries.
    /// Throws FileNotFoundException if the zip file does not exist,
    /// InvalidDataException for malformed zip files, and other IO exceptions as appropriate.
    /// </summary>
    public IReadOnlyList<ZipEntryInfo> ListContents(
        string path,
        ListOptions? options = null,
        IScanProgress? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (path is null) throw new ArgumentNullException(nameof(path));
        if (!File.Exists(path)) throw new FileNotFoundException("Zip file not found.", path);

        using var zip = ZipFile.OpenRead(path);
        var results = new List<ZipEntryInfo>(zip.Entries.Count);


        foreach (var entry in zip.Entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Ignore directory entries
            if (string.IsNullOrEmpty(entry.Name))
                continue;

            string raw = entry.FullName.Replace('\\', '/');

            if (!PathNormaliser.TryNormalise(raw, options?.StripRoot, out var normalised))
                continue;

            results.Add(new ZipEntryInfo(normalised, IsDirectory: false));
            progress?.FilesScanned(results.Count);
        }

        progress?.Completed(results.Count);
        return results;
    }
}
