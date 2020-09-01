
namespace netasloc.Web.Models.API
{
    public class AnalyzeRemoteRepositoriesRequest
    {
        public string WorkingDirectoryPath { get; set; }
        public string ResultsFolderName { get; set; }
        public string[] RemoteRepositories { get; set; }
    }
}
