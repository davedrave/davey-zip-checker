using System;
using System.ComponentModel;
using System.Net;
using NetworkConnection;

namespace Davey.ZipChecker.Core.Networking
{
    public class NetworkShareAuthenticationProvider : IPathAuthenticationProvider
    {
        private readonly string? _username;
        private readonly string? _password;
        private readonly string? _domain;

        public NetworkShareAuthenticationProvider(
            string? username = null,
            string? password = null,
            string? domain = null)
        {
            _username = username;
            _password = password;
            _domain = domain;
        }

        public IDisposable? GetAuthenticationScope(string path)
        {
            if (!IsNetworkPath(path))
                return null;

            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                return null;

            string shareRoot = GetShareRoot(path);

            // Create NetworkCredential
            var credentials = new NetworkCredential(_username, _password, _domain ?? string.Empty);

            try
            {
                // NetworkConnection constructor: (string networkName, NetworkCredential credentials)
                return new NetworkConnection.NetworkConnection(shareRoot, credentials);
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1219) // ERROR_SESSION_CREDENTIAL_CONFLICT
            {
                throw new InvalidOperationException(
                    $"Cannot connect to {shareRoot} - there's still an active connection with different credentials. " +
                    $"This should have been disconnected automatically. Please try running the command again.",
                    ex);
            }
        }

        private static bool IsNetworkPath(string path)
        {
            return !string.IsNullOrEmpty(path) &&
                   (path.StartsWith(@"\\") || path.StartsWith(@"//"));
        }

        private static string GetShareRoot(string uncPath)
        {
            var parts = uncPath.TrimStart('\\').Split('\\');
            if (parts.Length < 2)
                return uncPath;

            return $@"\\{parts[0]}\{parts[1]}";
        }
    }
}