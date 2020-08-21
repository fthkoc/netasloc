using System.IO;

namespace netasloc.Web.Models
{
    public class AnalyzeAllRepositoriesRequest
    {
        public string ResultsDirectory { get; set; }
        public string[] Repositories { get; set; }
    }
}
