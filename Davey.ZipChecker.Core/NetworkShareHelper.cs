using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Davey.ZipChecker.Core
{
    public static class NetworkShareHelper
    {
        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetAddConnection2(
            ref NETRESOURCE netResource,
            string password,
            string username,
            int flags);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetCancelConnection2(
            string name,
            int flags,
            bool force);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NETRESOURCE
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        private const int RESOURCETYPE_DISK = 0x00000001;

        public static void ConnectToShare(string sharePath, string username, string password)
        {
            var netResource = new NETRESOURCE
            {
                dwType = RESOURCETYPE_DISK,
                RemoteName = sharePath
            };

            int result = WNetAddConnection2(ref netResource, password, username, 0);

            if (result != 0)
            {
                throw new System.ComponentModel.Win32Exception(result,
                    $"Failed to connect to network share: {sharePath}");
            }
        }

        public static void DisconnectFromShare(string sharePath, bool force = false)
        {
            WNetCancelConnection2(sharePath, 0, force);
        }
    }
}
