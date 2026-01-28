using Davey.ZipChecker.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Davey.ZipChecker
{
    public sealed class FolderContentLister : IContentLister
    {
        /// <summary>
        /// Lists files in a directory recursively.
        /// Produces relative paths (relative to <paramref name="path"/>) using '/' as the separator.
        /// Directories are implicit via file paths and are not emitted.
        /// </summary>
        public IReadOnlyList<ZipEntryInfo> ListContents
        (
            string path,
            ListOptions? options = null,
            IScanProgress? progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory not found: {path}");

            progress?.Started("Scanning folder");
            
            int count = 0;
            string baseFull = Path.GetFullPath(path);
            var results = new List<ZipEntryInfo>();

            foreach (string full in Directory.EnumerateFiles(
                         baseFull, "*", SearchOption.AllDirectories))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Relative path from the chosen folder root
                string relative = Path
                    .GetRelativePath(baseFull, full)
                    .Replace('\\', '/');

                // Optional root stripping (same semantics as ZIP)
                if (!PathNormaliser.TryNormalise(relative, options?.StripRoot, out var normalised))
                    continue;

                count++;
                progress?.FilesScanned(count);


                results.Add(new ZipEntryInfo(
                    Path: relative,
                    IsDirectory: false));
            }

            progress?.Completed(count);
            return results;
        }
    }
}
