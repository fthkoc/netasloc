using netasloc.Core.DTO;
using System.Collections.Generic;

namespace netasloc.Core.Services
{
    public interface _IDataAccessService
    {
        DirectoryDTO GetByID(uint id);
        IEnumerable<DirectoryDTO> GetAll();
        bool Create(DirectoryDTO item);
        bool Update(uint id, DirectoryDTO item);
        bool Delete(uint id);

        IEnumerable<DirectoryDTO> GetLastAnalyzedDirectories();
    }
}
