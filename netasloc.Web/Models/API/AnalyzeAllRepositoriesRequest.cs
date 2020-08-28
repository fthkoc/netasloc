using System.IO;

namespace netasloc.Web.Models.API
{
    public class AnalyzeAllRepositoriesRequest
    {
        public string ResultsDirectory { get; set; }
        public string[] Repositories { get; set; }
    }
}
