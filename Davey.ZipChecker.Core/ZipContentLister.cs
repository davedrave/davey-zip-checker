using Davey.ZipChecker;
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
    public IReadOnlyList<ZipEntryInfo> ListContents(string path)
    {
        if (path is null) throw new ArgumentNullException(nameof(path));
        if (!File.Exists(path)) throw new FileNotFoundException("Zip file not found.", path);

        using var zip = ZipFile.OpenRead(path);
        var results = new List<ZipEntryInfo>(zip.Entries.Count);

        foreach (var entry in zip.Entries)
        {
            // Normalize separators to forward slash for consistent comparisons
            string normalized = entry.FullName.Replace('\\', '/');
            bool isDirectory = normalized.EndsWith("/");

            DateTimeOffset? lastWrite = null;
            // ZipArchiveEntry.LastWriteTime is a DateTimeOffset (non-nullable). check for default/min value.
            if (entry.LastWriteTime != default)
                lastWrite = entry.LastWriteTime;

            results.Add(new ZipEntryInfo(
                Path: normalized,
                IsDirectory: isDirectory
            ));
        }

        return results;
    }

    
}
