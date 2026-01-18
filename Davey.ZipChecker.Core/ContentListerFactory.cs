using System;
using System.IO;
using System.IO.Compression;

namespace Davey.ZipChecker
{
    /// <summary>
    /// Factory that creates an <see cref="IContentLister"/> appropriate for the provided path.
    /// - If the path is an existing directory, returns <see cref="FolderContentLister"/>.
    /// - If the path is an existing file and appears to be a ZIP, returns <see cref="ZipContentLister"/>.
    /// Throws ArgumentNullException for null input, FileNotFoundException if the path does not exist,
    /// or ArgumentException if the file is not a valid zip archive.
    /// </summary>
    public static class ContentListerFactory
    {
        public static IContentLister Create(string path)
        {
            if (path is null) throw new ArgumentNullException(nameof(path));

            if (Directory.Exists(path))
                return new FolderContentLister();

            if (File.Exists(path))
            {
                // Quick check by extension first
                var ext = Path.GetExtension(path);
                if (string.Equals(ext, ".zip", StringComparison.OrdinalIgnoreCase))
                    return new global::ZipContentLister();

                // If extension is not .zip, try to open it as a zip to be certain.
                try
                {
                    using var z = ZipFile.OpenRead(path);
                    return new global::ZipContentLister();
                }
                catch (InvalidDataException)
                {
                    throw new ArgumentException("File exists but is not a valid ZIP archive.", nameof(path));
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    // Surface IO/permission issues to the caller.
                    throw;
                }
            }

            throw new FileNotFoundException("Path does not exist.", path);
        }

        /// <summary>
        /// Convenience helper: create a lister for the path and run ListContents on it.
        /// </summary>
        public static IReadOnlyList<ZipEntryInfo> ListContentsFor(string path)
            => Create(path).ListContents(path);
    }
}