using netasloc.Data.Entity;
using System.Collections.Generic;

namespace netasloc.Data.DAO
{
    public interface _IDirectoryDAO : __IOperationsDAO<Directory>
    {
        IEnumerable<Directory> GetLastAnalyzedDirectories();
    }
}
