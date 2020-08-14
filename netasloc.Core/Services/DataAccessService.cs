using netasloc.Core.DTO;
using netasloc.Core.Mapping;
using netasloc.Data.DAO;
using netasloc.Data.Entity;
using System.Collections.Generic;

namespace netasloc.Core.Services
{
    public class DataAccessService : _IDataAccessService
    {
        private readonly _IOperationsDAO<Directory> _directories;
        private readonly _IMapper<Directory, DirectoryDTO> _directoryMapper;
        public DataAccessService(_IOperationsDAO<Directory> directories, _IMapper<Directory, DirectoryDTO> directoryMapper)
        {
            _directories = directories;
            _directoryMapper = directoryMapper;
        }

        public DirectoryDTO GetByID(uint id)
        {
            return _directoryMapper.DataToCore(_directories.GetByID(id));
        }

        public IEnumerable<DirectoryDTO> GetAll()
        {
            var result = _directories.GetAll();

            List<DirectoryDTO> response = new List<DirectoryDTO>();
            foreach (var item in result)
            {
                response.Add(_directoryMapper.DataToCore(item));
            }

            return response;
        }

        public bool Create(DirectoryDTO item)
        {
            return _directories.Create(_directoryMapper.CoreToData(item));
        }

        public bool Update(uint id, DirectoryDTO item)
        {
            return _directories.Update(id, _directoryMapper.CoreToData(item));
        }

        public bool Delete(uint id)
        {
            return _directories.Delete(id);
        }

        public IEnumerable<DirectoryDTO> GetLastAnalyzedDirectories()
        {
            var result = _directories.GetLastAnalyzedDirectories();

            List<DirectoryDTO> response = new List<DirectoryDTO>();
            foreach (var item in result)
            {
                response.Add(_directoryMapper.DataToCore(item));
            }

            return response;
        }
    }
}
