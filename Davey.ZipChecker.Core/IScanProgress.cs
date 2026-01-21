using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davey.ZipChecker.Core
{
    public interface IScanProgress
    {
        void Started(string description);
        void FilesScanned(int count);
        void Completed(int totalFiles);
    }
}
