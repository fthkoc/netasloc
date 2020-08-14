using netasloc.Core.DTO;
using netasloc.Core.Mapping;
using netasloc.Data.DAO;
using netasloc.Data.Entity;
using System.Collections.Generic;

namespace netasloc.Core.Services
{
    public class DataAccessService : _IDataAccessService
    {
        private readonly _IDirectoryDAO _directories;
        private readonly _IReleaseDAO _releases;
        private readonly _IMapper<Directory, DirectoryDTO> _directoryMapper;
        private readonly _IMapper<Release, ReleaseDTO> _releaseMapper;
        public DataAccessService(_IDirectoryDAO directories, _IMapper<Directory, DirectoryDTO> directoryMapper,
                                 _IReleaseDAO releases, _IMapper<Release, ReleaseDTO> releaseMapper)
        {
            _directories = directories;
            _directoryMapper = directoryMapper;
            _releases = releases;
            _releaseMapper = releaseMapper;
        }

        public DirectoryDTO GetDirectoryByID(uint id)
        {
            return _directoryMapper.DataToCore(_directories.GetByID(id));
        }

        public IEnumerable<DirectoryDTO> GetAllDirectories()
        {
            var result = _directories.GetAll();

            List<DirectoryDTO> response = new List<DirectoryDTO>();
            foreach (var item in result)
            {
                response.Add(_directoryMapper.DataToCore(item));
            }

            return response;
        }

        public bool CreateDirectory(DirectoryDTO item)
        {
            return _directories.Create(_directoryMapper.CoreToData(item));
        }

        public bool UpdateDirectory(uint id, DirectoryDTO item)
        {
            return _directories.Update(id, _directoryMapper.CoreToData(item));
        }

        public bool DeleteDirectory(uint id)
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

        public ReleaseDTO GetReleaseByID(uint id)
        {
            return _releaseMapper.DataToCore(_releases.GetByID(id));
        }

        public IEnumerable<ReleaseDTO> GetAllReleases()
        {
            var result = _releases.GetAll();

            List<ReleaseDTO> response = new List<ReleaseDTO>();
            foreach (var item in result)
            {
                response.Add(_releaseMapper.DataToCore(item));
            }

            return response;
        }

        public bool CreateRelease(ReleaseDTO item)
        {
            return _releases.Create(_releaseMapper.CoreToData(item));
        }

        public bool UpdateRelease(uint id, ReleaseDTO item)
        {
            return _releases.Update(id, _releaseMapper.CoreToData(item));
        }

        public bool DeleteRelease(uint id)
        {
            return _releases.Delete(id);
        }
    }
}
