using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker.Core.Networking
{
    public interface IPathAuthenticationProvider
    {
        /// <summary>
        /// Gets an authentication scope for the given path.
        /// Returns null if no authentication is needed.
        /// </summary>
        IDisposable? GetAuthenticationScope(string path);
    }
}
