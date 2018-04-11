using System;
using System.Collections.Generic;
using System.Text;

namespace Xandar.Service
{
    public interface ILocalFileHelper
    {
        string GetLocalFilePath(string filename);
    }
}
