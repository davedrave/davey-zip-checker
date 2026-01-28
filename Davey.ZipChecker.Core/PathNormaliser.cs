using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker.Core
{
    internal static class PathNormaliser
    {
        public static bool TryNormalise(
            string path,
            string? stripRoot,
            out string normalized)
        {
            normalized = path.Replace('\\', '/');

            if (string.IsNullOrEmpty(stripRoot))
                return true;

            stripRoot = stripRoot.Replace('\\', '/').TrimEnd('/') + "/";

            if (!normalized.StartsWith(stripRoot, StringComparison.OrdinalIgnoreCase))
                return false;

            normalized = normalized.Substring(stripRoot.Length);
            return true;
        }
    }

}
