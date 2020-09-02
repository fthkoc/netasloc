
namespace netasloc.Web.Models.API
{
    public class AnalyzeLocalRepositoriesRequest
    {
        public string ResultsDirectory { get; set; }
        public string[] Repositories { get; set; }
    }
}
