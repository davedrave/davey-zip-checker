using Davey.ZipChecker.Core;
using Davey.ZipChecker.Core.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker
{
    public interface IContentLister
    {
        public IReadOnlyList<ZipEntryInfo> ListContents
        (
            string path,
            ListOptions? options = null,
            IScanProgress? progress = null,
            CancellationToken cancellationToken = default
        );
    }
}
