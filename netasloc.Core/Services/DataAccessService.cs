﻿using Microsoft.Extensions.Logging;
using netasloc.Core.DTO;
using netasloc.Core.Mapping;
using netasloc.Data.DAO;
using netasloc.Data.Entity;
using System;
using System.Collections.Generic;

namespace netasloc.Core.Services
{
    public class DataAccessService : _IDataAccessService
    {
        private readonly ILogger<DataAccessService> _logger;
        private readonly _IDirectoryDAO _directories;
        private readonly _IReleaseDAO _releases;
        private readonly _IAnalyzeResultDAO _analyzeResults;
        private readonly _IMapper<Directory, DirectoryDTO> _directoryMapper;
        private readonly _IMapper<Release, ReleaseDTO> _releaseMapper;
        private readonly _IMapper<AnalyzeResult, AnalyzeResultDTO> _analyzeResultMapper;

        public DataAccessService(ILogger<DataAccessService> logger, 
            _IDirectoryDAO directories, _IMapper<Directory, DirectoryDTO> directoryMapper, 
            _IReleaseDAO releases, _IMapper<Release, ReleaseDTO> releaseMapper,
            _IAnalyzeResultDAO analyzeResults, _IMapper<AnalyzeResult, AnalyzeResultDTO> analyzeResultMapper)
        {
            _logger = logger;
            _directories = directories;
            _directoryMapper = directoryMapper;
            _releases = releases;
            _releaseMapper = releaseMapper;
            _analyzeResults = analyzeResults;
            _analyzeResultMapper = analyzeResultMapper;
        }

        public DirectoryDTO GetDirectoryByID(uint id)
        {
            _logger.LogInformation("DataAccessService::GetDirectoryByID::called.");
            DirectoryDTO response;
            try
            {
                response = _directoryMapper.DataToCore(_directories.GetByID(id));
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::GetDirectoryByID::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::GetDirectoryByID::finished.");
            return response;
        }

        public IEnumerable<DirectoryDTO> GetAllDirectories()
        {
            _logger.LogInformation("DataAccessService::GetAllDirectories::called.");
            List<DirectoryDTO> response = new List<DirectoryDTO>();
            try
            {
                var result = _directories.GetAll();
                foreach (var item in result)
                {
                    response.Add(_directoryMapper.DataToCore(item));
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::GetAllDirectories::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::GetAllDirectories::finished.");
            return response;
        }

        public bool CreateDirectory(DirectoryDTO item)
        {
            _logger.LogInformation("DataAccessService::CreateDirectory::called.");
            bool response;
            try
            {
                response = _directories.Create(_directoryMapper.CoreToData(item));
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::CreateDirectory::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::CreateDirectory::finished.");
            return response;
        }

        public bool UpdateDirectory(uint id, DirectoryDTO item)
        {
            _logger.LogInformation("DataAccessService::UpdateDirectory::called.");
            bool response;
            try
            {
                response = _directories.Update(id, _directoryMapper.CoreToData(item));
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::UpdateDirectory::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::UpdateDirectory::finished.");
            return response;
        }

        public bool DeleteDirectory(uint id)
        {
            _logger.LogInformation("DataAccessService::DeleteDirectory::called.");
            bool response;
            try
            {
                response = _directories.Delete(id);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::DeleteDirectory::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::DeleteDirectory::finished.");
            return response;
        }

        public IEnumerable<DirectoryDTO> GetLastAnalyzedDirectories()
        {
            _logger.LogInformation("DataAccessService::GetLastAnalyzedDirectories::called.");
            List<DirectoryDTO> response = new List<DirectoryDTO>();
            try
            {
                var result = _directories.GetLastAnalyzedDirectories();
                foreach (var item in result)
                {
                    response.Add(_directoryMapper.DataToCore(item));
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::GetLastAnalyzedDirectories::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::GetLastAnalyzedDirectories::finished.");
            return response;
        }

        public ReleaseDTO GetReleaseByID(uint id)
        {
            _logger.LogInformation("DataAccessService::GetReleaseByID::called.");
            ReleaseDTO response;
            try
            {
                response = _releaseMapper.DataToCore(_releases.GetByID(id));
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::GetReleaseByID::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::GetReleaseByID::finished.");
            return response;
        }

        public IEnumerable<ReleaseDTO> GetAllReleases()
        {
            _logger.LogInformation("DataAccessService::GetAllReleases::called.");
            List<ReleaseDTO> response = new List<ReleaseDTO>();
            try
            {
                var result = _releases.GetAll();
                foreach (var item in result)
                {
                    response.Add(_releaseMapper.DataToCore(item));
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::GetAllReleases::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::GetAllReleases::finished.");
            return response;
        }

        public bool CreateRelease(ReleaseDTO item)
        {
            _logger.LogInformation("DataAccessService::CreateRelease::called.");
            bool response;
            try
            {
                response = _releases.Create(_releaseMapper.CoreToData(item));
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::CreateRelease::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::CreateRelease::finished.");
            return response;
        }

        public bool UpdateRelease(uint id, ReleaseDTO item)
        {
            _logger.LogInformation("DataAccessService::UpdateRelease::called.");
            bool response;
            try
            {
                response = _releases.Update(id, _releaseMapper.CoreToData(item));
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::UpdateRelease::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::UpdateRelease::finished.");
            return response;
        }

        public bool DeleteRelease(uint id)
        {
            _logger.LogInformation("DataAccessService::DeleteRelease::called.");
            bool response;
            try
            {
                response = _releases.Delete(id);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::DeleteRelease::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::DeleteRelease::finished.");
            return response;
        }

        public AnalyzeResultDTO GetAnalyzeResultByID(uint id)
        {
            _logger.LogInformation("DataAccessService::GetAnalyzeResultByID::called.");
            AnalyzeResultDTO response;
            try
            {
                response = _analyzeResultMapper.DataToCore(_analyzeResults.GetByID(id));
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::GetAnalyzeResultByID::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::GetAnalyzeResultByID::finished.");
            return response;
        }

        public IEnumerable<AnalyzeResultDTO> GetAllAnalyzeResults()
        {
            _logger.LogInformation("DataAccessService::GetAllAnalyzeResults::called.");
            List<AnalyzeResultDTO> response = new List<AnalyzeResultDTO>();
            try
            {
                var result = _analyzeResults.GetAll();
                foreach (var item in result)
                {
                    response.Add(_analyzeResultMapper.DataToCore(item));
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::GetAllAnalyzeResults::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::GetAllAnalyzeResults::finished.");
            return response;
        }

        public bool CreateAnalyzeResult(AnalyzeResultDTO item)
        {
            _logger.LogInformation("DataAccessService::CreateAnalyzeResult::called.");
            bool response;
            try
            {
                response = _analyzeResults.Create(_analyzeResultMapper.CoreToData(item));
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::CreateAnalyzeResult::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::CreateAnalyzeResult::finished.");
            return response;
        }

        public bool UpdateAnalyzeResult(uint id, AnalyzeResultDTO item)
        {
            _logger.LogInformation("DataAccessService::UpdateAnalyzeResult::called.");
            bool response;
            try
            {
                response = _analyzeResults.Update(id, _analyzeResultMapper.CoreToData(item));
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::UpdateAnalyzeResult::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::UpdateAnalyzeResult::finished.");
            return response;
        }

        public bool DeleteAnalyzeResult(uint id)
        {
            _logger.LogInformation("DataAccessService::DeleteAnalyzeResult::called.");
            bool response;
            try
            {
                response = _analyzeResults.Delete(id);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::DeleteAnalyzeResult::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::DeleteAnalyzeResult::finished.");
            return response;
        }

        public IEnumerable<AnalyzeResultDTO> GetAnalyzeResultsForRelease(DateTime ReleaseStart, DateTime ReleaseEnd)
        {
            _logger.LogInformation("DataAccessService::GetAllAnalyzeResults::called.");
            List<AnalyzeResultDTO> response = new List<AnalyzeResultDTO>();
            try
            {
                var result = _analyzeResults.GetAnalyzeResultsForRelease(ReleaseStart, ReleaseEnd);
                foreach (var item in result)
                {
                    response.Add(_analyzeResultMapper.DataToCore(item));
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("DataAccessService::GetAllAnalyzeResults::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("DataAccessService::GetAllAnalyzeResults::finished.");
            return response;
        }
    }
}
