using netasloc.Core.DTO;
using System.Collections.Generic;

namespace netasloc.Web.Models.ViewModels
{
    public class AnalyzeDetailsViewModel
    {
        public AnalyzeDetailsViewModel()
        {
            Directories = new List<DirectoryDTO>();
        }

        public AnalyzeResultDTO AnalyzeResult { get; set; }
        public IList<DirectoryDTO> Directories { get; set; }
    }
}
