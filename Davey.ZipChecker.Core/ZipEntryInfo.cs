using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker
{
    public sealed record ZipEntryInfo(string Path, bool IsDirectory);
}
