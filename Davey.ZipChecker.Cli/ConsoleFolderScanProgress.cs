using Davey.ZipChecker.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker.Cli
{
    sealed class ConsoleScanProgress : IScanProgress
    {
        private readonly Stopwatch _sw = Stopwatch.StartNew();

        public void Started(string description)
        {
            Console.WriteLine(description);
            _sw.Restart();
        }

        public void FilesScanned(int count)
        {
            if (_sw.Elapsed.TotalSeconds >= 2)
            {
                Console.WriteLine($"  Scanning... {count:N0} files so far");
                _sw.Restart();
            }
        }

        public void Completed(int total)
            => Console.WriteLine($"Folder scan complete ({total:N0} files).");
    }
}
