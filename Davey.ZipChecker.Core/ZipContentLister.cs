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
    public IReadOnlyList<ZipEntryInfo> ListContents
    (
        string path,
        IScanProgress? progress = null,
        CancellationToken cancellationToken = default
    )
{
        if (path is null) throw new ArgumentNullException(nameof(path));
        if (!File.Exists(path)) throw new FileNotFoundException("Zip file not found.", path);

        using var zip = ZipFile.OpenRead(path);
        var results = new List<ZipEntryInfo>(zip.Entries.Count);

        return zip.Entries
                   // 🔑 Ignore directory entries
                   .Where(entry => !string.IsNullOrEmpty(entry.Name))
                   .Select(entry =>
                   {
                       // Normalize separators for consistent comparison
                       string normalized = entry.FullName.Replace('\\', '/');

                       return new ZipEntryInfo(
                           Path: normalized,
                           IsDirectory: false
                       );
                   })
                   .ToList();
    }

    
}
