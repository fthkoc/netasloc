using netasloc.Core.Models;
using System.Collections.Generic;

namespace netasloc.Core.Services
{
    public interface _ILOCService
    {
        LOCForAllResponse AnalyzeLOCForAll(IEnumerable<string> directoryFullPaths);
        LOCForDirectoryResponse AnalyzeLOCForDirectory(string directoryFullPath);
        LOCForSingleFileResponse AnalyzeLOCForSingleFile(string fileDirectory, string fileName, string fileExtension);
        bool WriteResultToFile(string directoryToWrite, LOCForAllResponse result);
        string[] GetRepositoriesFromGit(string WorkingDirectoryPath, string[] RemoteRepositories);
    }
}
