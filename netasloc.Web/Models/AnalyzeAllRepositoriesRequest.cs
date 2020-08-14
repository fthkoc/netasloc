using System.Collections.Generic;

namespace netasloc.Web.Models
{
    public class AnalyzeAllRepositoriesRequest
    {
        public AnalyzeAllRepositoriesRequest()
        {
            DirectoryList = new List<string>();
        }

        public IList<string> DirectoryList { get; set; }
    }
}
