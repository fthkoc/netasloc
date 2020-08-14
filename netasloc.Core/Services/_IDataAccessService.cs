using netasloc.Core.DTO;
using System.Collections.Generic;

namespace netasloc.Core.Services
{
    public interface _IDataAccessService
    {
        DirectoryDTO GetDirectoryByID(uint id);
        IEnumerable<DirectoryDTO> GetAllDirectories();
        bool CreateDirectory(DirectoryDTO item);
        bool UpdateDirectory(uint id, DirectoryDTO item);
        bool DeleteDirectory(uint id);

        IEnumerable<DirectoryDTO> GetLastAnalyzedDirectories();

        ReleaseDTO GetReleaseByID(uint id);
        IEnumerable<ReleaseDTO> GetAllReleases();
        bool CreateRelease(ReleaseDTO item);
        bool UpdateRelease(uint id, ReleaseDTO item);
        bool DeleteRelease(uint id);
    }
}
