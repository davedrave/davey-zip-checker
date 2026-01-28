using Davey.ZipChecker.Core;
using Davey.ZipChecker.Core.Networking;
using System;
using System.Collections.Generic;
using System.IO;

namespace Davey.ZipChecker
{
    public sealed class FolderContentLister : IContentLister
    {
        private readonly IPathAuthenticationProvider? _authProvider;

        /// <summary>
        /// Initializes a new instance of FolderContentLister.
        /// </summary>
        /// <param name="authProvider">Optional authentication provider for network shares. 
        /// If null, no explicit authentication is performed (uses current Windows session).</param>
        public FolderContentLister(IPathAuthenticationProvider? authProvider = null)
        {
            _authProvider = authProvider;
        }

        /// <summary>
        /// Lists files in a directory recursively.
        /// Produces relative paths (relative to <paramref name="path"/>) using '/' as the separator.
        /// Directories are implicit via file paths and are not emitted.
        /// </summary>
        public IReadOnlyList<ZipEntryInfo> ListContents
        (
            string path,
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

            // Use authentication scope only if auth provider was supplied
            using (_authProvider?.GetAuthenticationScope(baseFull))
            {
                foreach (string full in Directory.EnumerateFiles(
                             baseFull, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    count++;
                    progress?.FilesScanned(count);

                    string relative = Path
                        .GetRelativePath(baseFull, full)
                        .Replace('\\', '/');

                    results.Add(new ZipEntryInfo(
                        Path: relative,
                        IsDirectory: false));
                }
            }

            progress?.Completed(count);
            return results;
        }
    }
}