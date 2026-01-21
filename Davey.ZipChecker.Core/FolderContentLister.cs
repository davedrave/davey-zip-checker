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
        public IReadOnlyList<ZipEntryInfo> ListContents(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory not found: {path}");

            string baseFull = Path.GetFullPath(path);
            var results = new List<ZipEntryInfo>();

            foreach (string full in Directory.EnumerateFiles(
                         baseFull, "*", SearchOption.AllDirectories))
            {
                string relative = Path
                    .GetRelativePath(baseFull, full)
                    .Replace('\\', '/');

                results.Add(new ZipEntryInfo(
                    Path: relative,
                    IsDirectory: false));
            }

            return results;
        }
    }
}
