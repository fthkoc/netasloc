using netasloc.Core.DTO;
using System.Collections.Generic;

namespace netasloc.Web.Models.ViewModels
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Releases = new ReleaseData();
            Directories = new DirectoryData();
            AnalyzeResults = new AnalyzeResultData();
        }

        public ReleaseData Releases { get; set; }
        public DirectoryData Directories { get; set; }
        public AnalyzeResultData AnalyzeResults { get; set; }
    }

    public class ReleaseData
    {
        public ReleaseData()
        {
            Releases = new List<ReleaseDTO>();
        }

        public IEnumerable<ReleaseDTO> Releases { get; set; }
        public string releaseCodes { get; set; }
        public string totalLines { get; set; }
        public string totalCodeLines { get; set; }
        public string slocDifferences { get; set; }
        public string locDifferences { get; set; }
    }

    public class DirectoryData
    {
        public string projectNames { get; set; }
        public string fileCounts { get; set; }
        public string totalLines { get; set; }
        public string totalCodeLines { get; set; }
        public string totalCommentLines { get; set; }
        public string totalEmptyLines { get; set; }
        public string date { get; set; }
    }

    public class AnalyzeResultData
    {
        public AnalyzeResultData()
        {
            AnalyzeResults = new List<AnalyzeResultDTO>();
        }

        public IEnumerable<AnalyzeResultDTO> AnalyzeResults { get; set; }
        public string analyzeDates { get; set; }
        public string totalLines { get; set; }
        public string totalCodeLines { get; set; }
        public string slocDifferences { get; set; }
        public string locDifferences { get; set; }
    }
}
