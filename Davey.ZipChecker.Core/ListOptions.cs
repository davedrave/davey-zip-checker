using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker.Core
{
    public sealed class ListOptions
    {
        /// <summary>
        /// Optional path prefix to strip from entries (e.g. "package/")
        /// </summary>
        public string? StripRoot { get; init; }
    }
}
