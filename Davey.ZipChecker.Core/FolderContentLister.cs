using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker
{
    public class FolderContentLister : IContentLister
    {
        /// <summary>
        /// Lists the entries in a directory (recursively).
        /// Produces relative paths (relative to <paramref name="path"/>) using '/' as the separator.
        /// Directory entries will have a trailing '/' and IsDirectory will be true.
        /// Throws ArgumentNullException if directoryPath is null and DirectoryNotFoundException if it does not exist.
        /// </summary>
        public  IReadOnlyList<ZipEntryInfo> ListContents(string path)
        {
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"Directory not found: {path}");

            // Use full path for consistent relative-path calculations
            var baseFull = Path.GetFullPath(path);

            // Enumerate files and directories recursively. Include directories so empty directories are represented.
            var entries = Directory.EnumerateFiles(baseFull, "*", SearchOption.AllDirectories)
                                   .OrderBy(p => p, StringComparer.OrdinalIgnoreCase);

            var results = new List<ZipEntryInfo>();

            foreach (var full in entries)
            {
                // Compute a relative path and normalize separators to forward slash
                var relative = Path.GetRelativePath(baseFull, full).Replace('\\', '/');

                bool isDirectory = Directory.Exists(full);

                // Ensure directories end with a trailing slash to match zip-style entries
                if (isDirectory && !relative.EndsWith("/"))
                    relative += "/";

                results.Add(new ZipEntryInfo(relative, isDirectory));
            }

            return results;
        }
    }
}
