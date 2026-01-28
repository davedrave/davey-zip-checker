using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker.Core
{
    public static class ComparisonPathNormaliser
    {
        public static string Normalise(string path)
        {
            if (path is null)
                return string.Empty;

            // Normalise separators
            path = path.Replace('\\', '/');

            // Split into segments and normalise each
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries)
                            .Select(NormaliseSegment);

            return string.Join('/', parts);
        }

        private static string NormaliseSegment(string segment)
        {
            return segment
                .TrimEnd(' ')   // ZIP trailing space
                .TrimEnd('_');  // Windows substitute
        }
    }

}
