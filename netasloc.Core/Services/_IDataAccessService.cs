using netasloc.Core.DTO;
using System;
using System.Collections.Generic;

namespace netasloc.Core.Services
{
    public interface _IDataAccessService
    {
        // Directory
        DirectoryDTO GetDirectoryByID(uint id);
        IEnumerable<DirectoryDTO> GetAllDirectories();
        bool CreateDirectory(DirectoryDTO item);
        bool UpdateDirectory(uint id, DirectoryDTO item);
        bool DeleteDirectory(uint id);
        IEnumerable<DirectoryDTO> GetLastAnalyzedDirectories();

        // Release
        ReleaseDTO GetReleaseByID(uint id);
        IEnumerable<ReleaseDTO> GetAllReleases();
        bool CreateRelease(ReleaseDTO item);
        bool UpdateRelease(uint id, ReleaseDTO item);
        bool DeleteRelease(uint id);

        // AnalyzeResult
        AnalyzeResultDTO GetAnalyzeResultByID(uint id);
        IEnumerable<AnalyzeResultDTO> GetAllAnalyzeResults();
        bool CreateAnalyzeResult(AnalyzeResultDTO item);
        bool UpdateAnalyzeResult(uint id, AnalyzeResultDTO item);
        bool DeleteAnalyzeResult(uint id);
        IEnumerable<AnalyzeResultDTO> GetAnalyzeResultsForRelease(DateTime ReleaseStart, DateTime ReleaseEnd);
    }
}
