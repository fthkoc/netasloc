using netasloc.Core.Models;
using System.Collections.Generic;

namespace netasloc.Web.Models
{
    public class AnalyzeAllRepositoriesResponse
    {
        public AnalyzeAllRepositoriesResponse()
        {
            FileCount = 0;
            TotalLineCount = 0;
            CodeLineCount = 0;
            CommentLineCount = 0;
            EmptyLineCount = 0;
            DifferenceSLOC = 0;
            DifferenceLOC = 0;
            AllDirectoriesData = new Dictionary<string, LOCForDirectoryResponse>();
        }

        public uint FileCount { get; set; }
        public uint TotalLineCount { get; set; }
        public uint CodeLineCount { get; set; }
        public uint CommentLineCount { get; set; }
        public uint EmptyLineCount { get; set; }
        public int DifferenceSLOC { get; set; }
        public int DifferenceLOC { get; set; }

        public IDictionary<string, LOCForDirectoryResponse> AllDirectoriesData;
    }
}
